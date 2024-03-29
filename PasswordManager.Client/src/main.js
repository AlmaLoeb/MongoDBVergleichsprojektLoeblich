import axios from 'axios'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import store from './store.js'
import process from "node:process"

import './assets/main.css'
axios.defaults.baseURL = process.env.NODE_ENV == 'production' ? "/api" : "https://localhost:5000/api";

const app = createApp(App)
app.use(router)
app.use(store)
app.mount('#app')
