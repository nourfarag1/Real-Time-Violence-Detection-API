// This file must be in the root of the site
// It's a service worker for handling Firebase messaging

importScripts('https://www.gstatic.com/firebasejs/9.6.1/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/9.6.1/firebase-messaging-compat.js');

// IMPORTANT: You will need to paste your Firebase project's
// "firebaseConfig" object here. This is the public web configuration.
const firebaseConfig = {
    apiKey: "AIzaSyDWbfpCEFlLqujx5fE8zyhR6IOs2ImDd5c",
    authDomain: "notification-33c47.firebaseapp.com",
    projectId: "notification-33c47",
    storageBucket: "notification-33c47.firebasestorage.app",
    messagingSenderId: "958685724214",
    appId: "1:958685724214:web:dd887b473e28fd50ec2297",
    measurementId: "G-FYTRYEPZ5X"
};

// Initialize the Firebase app in the service worker
firebase.initializeApp(firebaseConfig);

// Retrieve an instance of Firebase Messaging so that it can handle background messages.
const messaging = firebase.messaging();

// If you want to handle background messages, you can add a handler here
messaging.onBackgroundMessage((payload) => {
  console.log(
    '[firebase-messaging-sw.js] Received background message ',
    payload,
  );
  // Customize notification here
  const notificationTitle = payload.notification.title;
  const notificationOptions = {
    body: payload.notification.body,
    icon: '/firebase-logo.png', // Optional: a path to an icon
  };

  self.registration.showNotification(notificationTitle, notificationOptions);
}); 