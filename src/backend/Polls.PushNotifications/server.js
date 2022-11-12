const express = require('express')
const cors = require('cors')
const bodyParser = require('body-parser')
const webpush = require('web-push')
const amqp = require('amqplib/callback_api');
var Connection = require('tedious').Connection;
var Request = require('tedious').Request

// CONFIG:
const {
    PORT = 4000,
    WEB_PUSH_PUBLIC_KEY = 'BJ5IxJBWdeqFDJTvrZ4wNRu7UY2XigDXjgiUBYEYVXDudxhEs0ReOJRBcBHsPYgZ5dyV8VjyqzbQKS8V7bUAglk',
    WEB_PUSH_PRIVATE_KEY = 'ERIZmc5T5uWGeRxedxu92k3HnpVwy_RCnQfgek1x2Y4',
    MS_SQL_SERVER = 'localhost',
    MS_SQL_USERNAME = 'sa',
    MS_SQL_PASSWORD = '5tgbNHY^',
    MS_SQL_DB = 'PollsDb',
    RABBIT_MQ_SERVER_URL = 'localhost',
    NUMBER_OF_SUBSCRIPTIONS_PER_BROADCAST = 3
  } = process.env;

// CONFIGURE SERVER:
const app = express()
app.use(cors())
app.use(bodyParser.json())
var publicChannel = null;

// ===================================================================================================================
// CONFIGURE WEB PUSH
// ===================================================================================================================
//setting our previously generated VAPID keys

webpush.setVapidDetails(
    'mailto:nenadkragovic@gmail.com',
    WEB_PUSH_PUBLIC_KEY,
    WEB_PUSH_PRIVATE_KEY
)

const broadcastMessage = (notification) => {
    try {
        getSubscriptions(notification.offset, NUMBER_OF_SUBSCRIPTIONS_PER_BROADCAST, function(err, subscriptions) {
            if (err !== null) {
                console.error('ERROR FETCHING ANY SUBSCRIPTIONS.');
                return;
            }
            if (subscriptions !== null) {
                subscriptions.forEach(subscription => {
                    try {
                        webpush.sendNotification(subscription, JSON.stringify(notification.message))
                        .then(() => {
                            console.log('NOTIFICATION SENT TO USER: ', data.UserId);
                        })
                        .catch((e) => {
                            console.log('NOTIFICATION IS NOT SENT TO USER: ', data.UserId);
                        });
                    }
                    catch(e) { console.log(e) }
                });
            }
            notification.offset += NUMBER_OF_SUBSCRIPTIONS_PER_BROADCAST;
            if (notification.offset < notification.total) {
                publicChannel.sendToQueue('buffer-exchange', Buffer.from(JSON.stringify(notification)));
            }
        });
    }
    catch(e) {
        console.error(e);
    }
}

const addMessageToBroadcastQueue = async (message) => {
    // TODO: perform action for each subscription
    getTotalNumberOfSubscriptions(function(err, total) {
        if (err !== null) {
            console.error('UNABLE TO GET SUBSCRITPIONS COUNT. MESSAGE WONT BE BROADCASTED.')
            return;
        }
        console.log('PUBLISHING BROADCAST MESSAGE TO QUEUE, TOTAL SUBSCRIPTIONS: ', total)
        publicChannel.sendToQueue('buffer-exchange', Buffer.from(JSON.stringify({total: total, offset: 0, message: message})));
    });
}

const pushMessageToUser = (data) => {
    getSubscriptionByUserId(data.UserId, function(err, subscription) {
        if (err !== null) {
            console.error('UNABLE TO SEND NOTIFICATION TO: ', data.UserId)
        }
        else {
            try {
                webpush.sendNotification(subscription, JSON.stringify({Message: data.Message, Title: data.Title}))
                .then(() => {
                    console.log('NOTIFICATION SENT TO USER: ', data.UserId);
                })
                .catch((e) => {
                    console.log('NOTIFICATION IS NOT SENT TO USER: ', data.UserId);
                });
            }
            catch (e) {
                console.log(e);
            }
        }
    });
}

// ===================================================================================================================
// CONNECT TO MS SQL DB
// ===================================================================================================================

var dbConfig = {
    server: MS_SQL_SERVER,
    authentication: {
        type: 'default',
        options: {
            userName: MS_SQL_USERNAME,
            password: MS_SQL_PASSWORD,
            trustServerCertificate: true
        }
    },
    options: {
        encrypt: false,
        database: MS_SQL_DB
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
    executeSQL(`SELECT * FROM [dbo].[PushNotificationSubscriptions] WHERE UserId='${userId}'`, (err, data) => {

        var sql = "INSERT INTO [dbo].[PushNotificationSubscriptions] ([Endpoint],[P246dhKey],[AuthKey],[UserId])" +
        `VALUES('${subscription.endpoint}', '${subscription.keys.p256dh}', '${subscription.keys.auth}', '${userId}');`;

        if (data != null && data !== undefined) {
            let row = data[0];
            if (row !== undefined) {
                console.warn(`SUBSCRIPTION FOR USER: ${userId} ALREADY EXISTS, IT WILL BE UPDATED.`);
                var sql = "UPDATE [dbo].[PushNotificationSubscriptions] SET " +
                `Endpoint='${subscription.endpoint}', P246dhKey='${subscription.keys.p256dh}', AuthKey='${subscription.keys.auth}';`;
            }
        }

        executeSQL(sql, (err, data) => {
            return callback(err, data);
        });
    });
}

// ===================================================================================================================
// CONNECT TO RABBIT MQ
// ===================================================================================================================
amqp.connect(`amqp://${RABBIT_MQ_SERVER_URL}`, function (error0, connection) {
    if (error0) {
        throw error0;
    }

    connection.createChannel(function (error1, channel) {
        if (error1) {
            throw error1;
        }

        console.log('Connected to Rabbit MQ');
        publicChannel = channel;

        channel.assertExchange('push-notifications-exchange', 'direct', { durable: false });
        channel.assertQueue('push-notifications-exchange', { durable: true });
        channel.bindQueue('push-notifications-exchange', 'push-notifications-exchange', '');
        channel.consume('push-notifications-exchange', function (msg) {
            try
            {
                if (msg.content) {
                    console.log("CONSUMED MESSAGE FROM API:", msg.content.toString());
                    let data = JSON.parse(msg.content.toString());
                    if (data.SendToAll) {
                        addMessageToBroadcastQueue(data);
                    }
                    else {
                        pushMessageToUser(data);
                    }
                }
            }
            catch (e) {
                console.log(e);
            }
        }, {
            noAck: true
        });

        channel.assertExchange('buffer-exchange', 'direct', { durable: true});
        channel.assertQueue('buffer-exchange', { durable: true });
        channel.bindQueue('buffer-exchange', 'buffer-exchange', '');
        channel.consume('buffer-exchange', function (msg) {
            try
            {
                if (msg.content) {
                    console.log("CONSUMED BUFFER: %s", msg.content.toString());
                    let notification = JSON.parse(msg.content.toString());
                    broadcastMessage(notification);
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
    saveSubscriptionToDbForUser(subscription, userId, function(err, data) {
        if (err !== null) {
            res.status(400)
            res.json({ error: err })
        }
        else {
            console.log('SUBSCRIPTION CREATED FOR USER: ', userId)
            res.status(201);
            res.json({ record: data })
        }
    })
})

app.post('/broadcast-notification', async (req, res) => {
    const message = req.body;
    await addMessageToBroadcastQueue(message)
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
            try {
                webpush.sendNotification(subscription, JSON.stringify(message));
                console.log('NOTIFICATION SENT TO USER: ', data.UserId);
            }
            catch (e) {
                console.log(e);
            }
            res.status(200);
            res.json({ record: subscription })
        }
    });
})

app.get('/get-subscriptions', async (req, res) => {
    getSubscriptions(0, 5, function(result) { res.json({ record: result }) })
})

// START SERVER
app.listen(PORT, () => console.log(`Notifications server listening on port ${PORT}!`))