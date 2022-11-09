const express = require('express')
const cors = require('cors')
const bodyParser = require('body-parser')
const webpush = require('web-push')
const amqp = require('amqplib/callback_api');
var Connection = require('tedious').Connection;
var Request = require('tedious').Request
const ConsumerQueue = require('consumer-queue');

const app = express()

app.use(cors())
app.use(bodyParser.json())
const port = 4000

// ===================================================================================================================
// CONFIGURE WEB PUSH
// ===================================================================================================================
//setting our previously generated VAPID keys
var config = {
    publicKey:'BJ5IxJBWdeqFDJTvrZ4wNRu7UY2XigDXjgiUBYEYVXDudxhEs0ReOJRBcBHsPYgZ5dyV8VjyqzbQKS8V7bUAglk',
    privateKey: 'ERIZmc5T5uWGeRxedxu92k3HnpVwy_RCnQfgek1x2Y4',
};

webpush.setVapidDetails(
    'mailto:nenadkragovic@gmail.com',
    config.publicKey,
    config.privateKey
)

/// MESSAGES QUEUE. IF THERE IS A HUGE LOAD MESSAGES SHOULD BE DISPATCHED IN BATCHES
const messagesToBroadcastQueue = new ConsumerQueue();
const numberOfSubscriptionsPerRun = 3;
var queueRunning = false;

function loop() {
    try {
        queueRunning = true;
        console.info('NOTIFICATION CONSUMED.')
        return messagesToBroadcastQueue.pop().then((notification) => {
            getSubscriptions(notification.offset, numberOfSubscriptionsPerRun, function(err, subscriptions) {
                if (err !== null) {
                    console.error('ERROR FETCHING SUBSCRIPTIONS ANY SUBSCRIPTIONS.');
                    return;
                }
                if (subscriptions !== null) {
                    subscriptions.forEach(subscription => {
                        try {
                            webpush.sendNotification(subscription, JSON.stringify(notification.message));
                        }
                        catch(e) { console.log(e) }
                    });
                }
                notification.offset += numberOfSubscriptionsPerRun;
                if (notification.offset < notification.total) {
                    messagesToBroadcastQueue.push(notification)
                    queueRunning = false;
                    return loop();
                }
            });
        });
    }
    catch(e) {
        console.error(e);
        queueRunning = false;
    }
}

const broadcastNotifications = async (message) => {
    // TODO: perform action for each subscription
    getTotalNumberOfSubscriptions(function(err, total) {
        if (err !== null) {
            console.error('UNABLE TO GET SUBSCRITPIONS COUNT. MESSAGE WONT BE BROADCASTED.')
            return;
        }
        console.log('TOTAL: ', total)
        messagesToBroadcastQueue.push({total: total, offset: 0, message: message})
        if (!queueRunning)
            loop();
    });
}

// ===================================================================================================================
// CONNECT TO MS SQL DB
// ===================================================================================================================
var TYPES = require('tedious').TYPES;
var dbConfig = {
    server: 'localhost',
    authentication: {
        type: 'default',
        options: {
            userName: 'sa',
            password: '5tgbNHY^',
            trustServerCertificate: true
        }
    },
    options: {
        encrypt: false,
        database: 'PollsDb'
    }
};

/*
* callback(error, records)
*/
const executeSQL = (sql, callback) => {
    let connection = new Connection(dbConfig);

    connection.connect((err) => {
        if (err)
            return callback(err, null);

        const request = new Request(sql, (err, rowCount, rows) => {
            connection.close();
            if (err) {
                console.log('Req failed: ', err)
                return callback(err, null);
            }
            callback(null, result);
        });

        var result = [];
        request.on('row', function(columns) {
            let columnData = []
            columns.forEach(function(column) {
                if (column.value !== null) {
                  columnData.push(column.value)
                }
              });
            result.push(columnData);
        });

        request.on('done', function(rowCount, more) {
            console.log(rowCount + ' rows returned');
            return callback(result);
        });

        connection.execSql(request);
    });
};

// REPOSITORY METHODS
const getTotalNumberOfSubscriptions = async (callback) => {
    var sql = `SELECT COUNT(*) FROM PushNotificationSubscriptions;`;
    executeSQL(sql, (err, data) => {
        if (data !== undefined) {
            let row = data[0];
            if (row !== undefined){
                return callback(err, row[0]);
            }
        }
        return callback(err, 'no subscriptions found!');
    });
}

const getSubscriptions = async (offset, limit, callback) => {
    var sql = `SELECT Endpoint, P246dhKey, AuthKey FROM PushNotificationSubscriptions ORDER BY Id OFFSET ${offset} ROWS FETCH NEXT ${limit} ROWS ONLY;`;
    executeSQL(sql, (err, data) => {
        if (data !== undefined) {
            var subscriptions = [];
            for (var i in data) {
                let row = data[i];
                if (row !== undefined) {
                    var subscription = {
                        endpoint: row[0],
                        expirationTime: null,
                        keys: {
                          p256dh: row[1],
                          auth: row[2]
                        }
                    };
                    subscriptions.push(subscription)
                }
            }
            return callback(err, subscriptions);
        }
        return callback(err, 'no subscriptions found!');
    });
}

const getSubscriptionByUserId = async (userId, callback) => {
    var sql = `select Endpoint, P246dhKey, AuthKey from [dbo].[PushNotificationSubscriptions] where UserId = '${userId}';`;
    executeSQL(sql, (err, data) => {
        if (data !== undefined) {
            let row = data[0];
            if (row !== undefined){
                var subscription = {
                    endpoint: row[0],
                    expirationTime: null,
                    keys: {
                      p256dh: row[1],
                      auth: row[2]
                    }
                };
                return callback(err, subscription);
            }
        }
        return callback(err, 'no subscriptions found!');
    });
}

const saveSubscriptionToDbForUser = (subscription, userId, callback) => {
    var sql = "INSERT INTO [dbo].[PushNotificationSubscriptions] ([Endpoint],[P246dhKey],[AuthKey],[UserId])" +
    `VALUES('${subscription.endpoint}', '${subscription.keys.p256dh}', '${subscription.keys.auth}', '${userId}');`;

    executeSQL(sql, (err, data) => {
        return callback(err, data);
    });
}

// ===================================================================================================================
// CONNECT TO RABBIT MQ
// ===================================================================================================================
amqp.connect('amqp://localhost', function (error0, connection) {
    if (error0) {
        throw error0;
    }

    connection.createChannel(function (error1, channel) {
        if (error1) {
            throw error1;
        }

        console.log('Connected to Rabbit MQ');

        channel.assertExchange('push-notifications-exchange', 'direct', { durable: false });
        channel.assertQueue('push-notifications-exchange', { durable: true });
        channel.bindQueue('push-notifications-exchange', 'push-notifications-exchange', '');
        channel.consume('push-notifications-exchange', function (msg) {
            try
            {
                if (msg.content) {
                    console.log(" [x] %s", msg.content.toString());
                    const message = msg.content.toString();
                    let data = JSON.parse(message);
                    if (data.SendToAll) {
                        broadcastNotifications(data);
                    }
                    else {
                        getSubscriptionByUserId(data.UserId, function(err, subscription) {
                            if (err !== null) {
                                console.error('UNABLE TO SEND NOTIFICATION TO: ', data.UserId)
                            }
                            else {
                                webpush.sendNotification(subscription, JSON.stringify({Message: data.Message, Title: data.Title}));
                                console.log('NOTIFICATION SENT TO USER: ', data.UserId);
                            }
                        });
                    }
                }
            }
            catch (e) {
                console.log(e);
            }
        }, {
            noAck: true
        });
    });
});

// ===================================================================================================================
// ADD ROUTES
// ===================================================================================================================
app.get('/', (req, res) =>
    res.send('Polls notifications API. Add subscription to API and device will be notified with fresh inforamtion!'))

// The new /save-subscription endpoint
app.post('/save-subscription', async (req, res) => {
    const subscription = req.body;
    let userId = req.query.userId;
    console.log('Subscription created for user: ', userId)
    saveSubscriptionToDbForUser(subscription, userId, function(err, data) {
        if (err !== null) {
            res.status(400)
            res.json({ error: err })
        }
        else {
            res.status(201);
            res.json({ record: data })
        }
    })
})

app.post('/broadcast-notification', async (req, res) => {
    const message = req.body;
    await broadcastNotifications(message)
    res.json({ message: 'message sent' })
})

app.post('/send-notification-to-user', async (req, res) => {
    const message = req.body;
    let userId = req.query.userId;
    getSubscriptionByUserId(userId, function(err, subscription) {
        if (err !== null) {
            res.status(400)
            res.json({ error: err })
        }
        else {
            console.log('NOTIFICATION SENT TO USER: ', userId)
            webpush.sendNotification(subscription, JSON.stringify(message));
            res.status(200);
            res.json({ record: subscription })
        }
    });
})

app.get('/get-subscriptions', async (req, res) => {
    getSubscriptions(0, 5, function(result) { res.json({ record: result }) })
})

app.listen(port, () => console.log(`Notifications server listening on port ${port}!`))