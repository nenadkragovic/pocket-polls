/* eslint-disable no-restricted-globals */

// This service worker can be customized!
// See https://developers.google.com/web/tools/workbox/modules
// for the list of available Workbox modules, or add any other
// code you'd like.
// You can also remove this file if you'd prefer not to use a
// service worker, and the Workbox build step will be skipped.

import { clientsClaim } from 'workbox-core';
import { ExpirationPlugin } from 'workbox-expiration';
import { precacheAndRoute, createHandlerBoundToURL } from 'workbox-precaching';
import { registerRoute } from 'workbox-routing';
import { StaleWhileRevalidate } from 'workbox-strategies';

clientsClaim();

const {
  WEB_PUSH_PUBLIC_KEY = 'BJ5IxJBWdeqFDJTvrZ4wNRu7UY2XigDXjgiUBYEYVXDudxhEs0ReOJRBcBHsPYgZ5dyV8VjyqzbQKS8V7bUAglk',
  NOTIFICATIONS_API_URL = 'https://www.pocket-polls.click:4000'
} = process.env

// Precache all of the assets generated by your build process.
// Their URLs are injected into the manifest variable below.
// This variable must be present somewhere in your service worker file,
// even if you decide not to use precaching. See https://cra.link/PWA
precacheAndRoute(self.__WB_MANIFEST);

// Set up App Shell-style routing, so that all navigation requests
// are fulfilled with your index.html shell. Learn more at
// https://developers.google.com/web/fundamentals/architecture/app-shell
const fileExtensionRegexp = new RegExp('/[^/?]+\\.[^/]+$');
registerRoute(
  // Return false to exempt requests from being fulfilled by index.html.
  ({ request, url }) => {
    // If this isn't a navigation, skip.
    if (request.mode !== 'navigate') {
      return false;
    } // If this is a URL that starts with /_, skip.

    if (url.pathname.startsWith('/_')) {
      return false;
    } // If this looks like a URL for a resource, because it contains // a file extension, skip.

    if (url.pathname.match(fileExtensionRegexp)) {
      return false;
    } // Return true to signal that we want to use the handler.

    return true;
  },
  createHandlerBoundToURL(process.env.PUBLIC_URL + '/index.html')
);

// An example runtime caching route for requests that aren't handled by the
// precache, in this case same-origin .png requests like those from in public/
registerRoute(
  // Add in any other file extensions or routing criteria as needed.
  ({ url }) => url.origin === self.location.origin && url.pathname.endsWith('.png'), // Customize this strategy as needed, e.g., by changing to CacheFirst.
  new StaleWhileRevalidate({
    cacheName: 'images',
    plugins: [
      // Ensure that once this runtime cache reaches a maximum size the
      // least-recently used images are removed.
      new ExpirationPlugin({ maxEntries: 50 }),
    ],
  })
);

// This allows the web app to trigger skipWaiting via
// registration.waiting.postMessage({type: 'SKIP_WAITING'})
self.addEventListener('message', (event) => {
  if (event.data && event.data.type === 'SKIP_WAITING') {
    self.skipWaiting();
  }
});

self.addEventListener('fetch', (event) => {
  if (event.request.mode === 'navigate') {
    event.respondWith(caches.match('/index.html'));
  } else {
    // Your other response logic goes here.
  }
});

// Any other custom service worker logic can go here.

// urlB64ToUint8Array is a magic function that will encode the base64 public key
// to Array buffer which is needed by the subscription option
const urlB64ToUint8Array = base64String => {
  const padding = '='.repeat((4 - (base64String.length % 4)) % 4)
  const base64 = (base64String + padding).replace(/\-/g, '+').replace(/_/g, '/')
  const rawData = atob(base64)
  const outputArray = new Uint8Array(rawData.length)
  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i)
  }
  return outputArray
}

const createOrUpdatePushSubscription = async () => {
  try {
    const applicationServerKey = urlB64ToUint8Array(WEB_PUSH_PUBLIC_KEY)
    const options = { applicationServerKey, userVisibleOnly: true }
    const subscription = await self.registration.pushManager.subscribe(options)

    let userId = new URL(location).searchParams.get('userId');
    const SERVER_URL = `${NOTIFICATIONS_API_URL}/save-subscription?userId=${userId}`;

    var response = await fetch(SERVER_URL, {
      method: 'post',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(subscription),
    })

    if (response.status === 201) {
      console.info('SUBSCRIBED TO PUSH NOTIFICATIONS SERVER!')
    }
    else {
      console.err('FAILED TO SUBSCRIBE TO PUSH NOTIFICATIONS SERVER!')
    }

  } catch (err) {
    console.error('Error', err)
  }
}

self.addEventListener('activate', async () => {
  // This will be called only once when the service worker is installed for first time.
    await createOrUpdatePushSubscription()
})

self.addEventListener('pushsubscriptionchange ', async () => {
    await createOrUpdatePushSubscription()
})

self.addEventListener('push', function(event) {
  if (event.data) {
    console.log('Push event: ', event.data.text())
    let data = JSON.parse(event.data.text());
    showLocalNotification(data.Title, data.Message, self.registration)
  }
})


const showLocalNotification = (title, body, swRegistration) => {
  const options = {
    body,
    icon: `${process.env.PUBLIC_URL}/logo192.png`,
    vibrate: true
  }
  swRegistration.showNotification(title, options)
}

// KEEP ALIVE SERVICE WORKER
setInterval(function(){
  fetch('/ping.txt')
}, 8000)