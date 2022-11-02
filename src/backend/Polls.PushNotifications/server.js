const express = require('express')
const cors = require('cors')
const bodyParser = require('body-parser')
const webpush = require('web-push')
const amqp = require('amqplib/callback_api');
const sql = require('mssql')

const app = express()

app.use(cors())
app.use(bodyParser.json())
const port = 4000

var config = {
    user: 'polls-api',
    password: '5tgbNHY^',
    server: '(localdb)\\Local',
    database: 'PollsDb1',
    publicKey:'BJ5IxJBWdeqFDJTvrZ4wNRu7UY2XigDXjgiUBYEYVXDudxhEs0ReOJRBcBHsPYgZ5dyV8VjyqzbQKS8V7bUAglk',
    privateKey: 'ERIZmc5T5uWGeRxedxu92k3HnpVwy_RCnQfgek1x2Y4',
};

app.get('/', (req, res) => res.send('Polls notifications API. Add subscription to API and device will be notified with fresh inforamtion!'))

// The new /save-subscription endpoint
app.post('/save-subscription', async (req, res) => {
    const subscription = req.body;
    addSubscription(subscription, function(result) { res.json({ record: result }) })
})

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

//route to test send notification
app.get('/send-notification', async (req, res) => {
    const message = 'TEST MESSAGE FROM PUSH API';
    await broadcastNotifications(message)
    res.json({ message: 'message sent' })
})

app.listen(port, () => console.log(`Example app listening on port ${port}!`))

amqp.connect('amqp://localhost', function (error0, connection) {
    if (error0) {
        throw error0;
    }
    connection.createChannel(function (error1, channel) {
        if (error1) {
            throw error1;
        }

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


const getSubscriptions = async (offset, limit, callback) => {
    try {
        // make sure that any items are correctly URL encoded in the connection string
        sql.connect(config, function (err) {

            if (err) console.log(err);

            // create Request object
            var request = new sql.Request();

            // query to the database and get the records
            request.query(`select * from PushNotificationSubscriptions order by Id offset ${offset} rows fetch next ${limit} rows only`,
                 function (err, recordset) {
                    if (err) {
                        console.log(err)
                        callback(null);
                    }
                    callback(recordset);
                });
        });
    } catch (err) {
        console.log(err);
        callback(null);
    }
}

const addSubscription = async (subscription, callback) => {
    try {
        // make sure that any items are correctly URL encoded in the connection string
        sql.connect(config, function (err) {

            if (err) console.log(err);

            // create Request object
            var request = new sql.Request();

            // query to the database and get the records
            request.query(`insert into PushNotificationSubscriptions(Endpoint, ExpirationTime, P246dhKey, AuthKey) ` +
            `values (${subscription.endpoint}, CURRENT_TIMESTAMP, ${subscription.p256dh}, ${subscription.auth})`,
                 function (err, recordset) {
                    if (err) {
                        console.log(err)
                        callback(null);
                    }
                    callback(recordset);
                });
        });
    } catch (err) {
        console.log(err);
        callback(null);
    }
}