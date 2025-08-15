document.addEventListener('DOMContentLoaded', () => {
    // -------------------------------------------------------------------
    // PASTE YOUR FIREBASE WEB APP CONFIG OBJECT HERE
    // You can get this from the Firebase console for your web app.
    // -------------------------------------------------------------------
    const firebaseConfig = {
        apiKey: "AIzaSyDWbfpCEFlLqujx5fE8zyhR6IOs2ImDd5c",
        authDomain: "notification-33c47.firebaseapp.com",
        projectId: "notification-33c47",
        storageBucket: "notification-33c47.firebasestorage.app",
        messagingSenderId: "958685724214",
        appId: "1:958685724214:web:dd887b473e28fd50ec2297",
        measurementId: "G-FYTRYEPZ5X"
    };
    // -------------------------------------------------------------------

    const tokenButton = document.getElementById('get-token-button');
    const tokenTextarea = document.getElementById('fcm-token');
    const statusEl = document.getElementById('status');
    
    // Disable the button initially
    tokenButton.disabled = true;

    if ('serviceWorker' in navigator) {
        // Register the service worker
        navigator.serviceWorker.register('/firebase-messaging-sw.js')
            .then((registration) => {
                console.log('Service Worker registration successful with scope: ', registration.scope);

                // Now, wait for the service worker to be ready and activated.
                return navigator.serviceWorker.ready;
            })
            .then((registration) => {
                console.log('Service Worker is active and ready.');
                statusEl.textContent = 'Ready. Click the button to get your token.';
                statusEl.className = 'success';
                
                // Initialize Firebase AFTER the service worker is active.
                firebase.initializeApp(firebaseConfig);
                const messaging = firebase.messaging();

                // Enable the button now that everything is truly ready.
                tokenButton.disabled = false;
                tokenButton.addEventListener('click', () => getToken(messaging));
            })
            .catch((err) => {
                console.error('Service Worker setup failed: ', err);
                statusEl.textContent = 'Service Worker setup failed. See browser console for details.';
                statusEl.className = 'error';
            });
    } else {
        statusEl.textContent = 'Service Workers are not supported in this browser.';
        statusEl.className = 'error';
    }

    function getToken(messaging) {
        console.log('Requesting permission...');
        tokenButton.disabled = true;
        statusEl.textContent = 'Requesting permission...';
        statusEl.className = '';

        Notification.requestPermission().then((permission) => {
            if (permission === 'granted') {
                console.log('Notification permission granted.');
                statusEl.textContent = 'Permission granted! Retrieving token...';
                statusEl.className = 'success';
                
                // Get registration token.
                messaging.getToken({ vapidKey: 'BEDtAG0cBW71T-e-YXqbb9pZ-NYter-r3DCaLt-u95wIg9Z_K40TgCcitjl4kD51Ig9EWn8aXnRWhzrepjEygCw' })
                    .then((currentToken) => {
                        if (currentToken) {
                            console.log('FCM Token:', currentToken);
                            tokenTextarea.value = currentToken;
                            statusEl.textContent = 'Token generated successfully!';
                            statusEl.className = 'success';
                        } else {
                            console.log('No registration token available. Request permission to generate one.');
                            statusEl.textContent = 'Failed to get token. Please ensure you have granted notification permissions.';
                            statusEl.className = 'error';
                            tokenButton.disabled = false;
                        }
                    }).catch((err) => {
                        console.error('An error occurred while retrieving token. ', err);
                        statusEl.textContent = 'Error retrieving token: ' + err.message;
                        statusEl.className = 'error';
                        tokenButton.disabled = false;
                    });
            } else {
                console.log('Unable to get permission to notify.');
                statusEl.textContent = 'Permission denied. You will not receive notifications.';
                statusEl.className = 'error';
            }
        });
    }
}); 