// main.js
import { createApp } from 'vue';
import App from './App.vue';
import axios from 'axios';
import { createRouter, createWebHistory } from 'vue-router';
import GroupListComponent from './components/GroupListComponent.vue';

// Configure axios
axios.defaults.baseURL = 'https://localhost:5000'; // Use the port where your backend API is running

// Define the routes for your application
import GroupDetailComponent from './components/GroupDetailComponent.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: GroupListComponent },
    {
      path: '/groups/:groupId',
      component: GroupDetailComponent,
      name: 'group-detail',
      props: true // This will pass route.params to the component as props
    },
    // ... other routes
  ],
});

// Create a new Vue application and use the router
const app = createApp(App);
app.use(router);
app.mount('#app');
