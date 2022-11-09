const express = require('express')
const cors = require('cors')
const bodyParser = require('body-parser')
const webpush = require('web-push')
const amqp = require('amqplib/callback_api');
var Connection = require('tedious').Connection;
var Request = require('tedious').Request

const app = express()

app.use(cors())
app.use(bodyParser.json())
const port = 4000

let subscriptionsData = new Object();

var config = {
    publicKey:'BJ5IxJBWdeqFDJTvrZ4wNRu7UY2XigDXjgiUBYEYVXDudxhEs0ReOJRBcBHsPYgZ5dyV8VjyqzbQKS8V7bUAglk',
    privateKey: 'ERIZmc5T5uWGeRxedxu92k3HnpVwy_RCnQfgek1x2Y4',
};

// CONFIGURE WEB PUSH
//setting our previously generated VAPID keys
webpush.setVapidDetails(
    'mailto:nenadkragovic@gmail.com',
    config.publicKey,
    config.privateKey
)

const broadcastNotifications = async (dataToSend) => {
    // TODO: perform action for each subscription
    getSubscriptions(0, 100, function(subscriptions) {
        if (subscriptions != null)
            subscriptions.forEach(subscription => {
                webpush.sendNotification(subscription, dataToSend);
            });
    });

}

// CONNECT TO MS SQL DB
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
            console.log("uspeo"),
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

// CONECT TO RABBIT MQ
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
                    broadcastNotifications(message)
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

// ADD ROUTES
app.get('/', (req, res) => res.send('Polls notifications API. Add subscription to API and device will be notified with fresh inforamtion!'))

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
            res.status(201);
            res.json({ record: data })
        }
    })
})

//route to test send notification
app.get('/send-notification', async (req, res) => {
    const message = 'TEST MESSAGE FROM PUSH API';
    await broadcastNotifications(message)
    res.json({ message: 'message sent' })
})

app.get('/get-subscriptions', async (req, res) => {
    getSubscriptions(0, 5, function(result) { res.json({ record: result }) })
})

app.listen(port, () => console.log(`Notifications server listening on port ${port}!`))

// OTHER METHODS
const getSubscriptions = async (offset, limit, callback) => {
    let subs = [];
    for(var key in subscriptionsData) {
        subs.push(subscriptionsData[key]);
      }

      executeSQL("SELECT * from [dbo].[Users]", (err, data) => {
        if (err)
          console.error(err);
        console.log(data);
        callback(data)
        return;
      });
    return;
}

const saveSubscriptionToDbForUser = (subscription, userId, callback) => {
    var sql = "INSERT INTO [dbo].[PushNotificationSubscriptions] ([Endpoint],[P246dhKey],[AuthKey],[UserId])" +
    `VALUES('${subscription.endpoint}',  '${subscription.keys.p256dh}', '${subscription.keys.auth}', '${userId}');`;

    executeSQL(sql, (err, data) => {
        return callback(err, data);
    });
}