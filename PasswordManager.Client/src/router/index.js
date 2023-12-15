import { createRouter, createWebHistory } from 'vue-router'
import store from '../store.js'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'home',
            component: HomeView,
        },
        {
            path: '/login',
            name: 'login',
            component: () => import('../views/LoginView.vue')
        },
        {
            path: '/password/:id',
            name: 'password',
            component: () => import('../views/PasswordView.vue')
        },
        {
            path: '/group/:id',
            name: 'group',
              meta: { authorize: true },
            component: () => import('../views/GroupView.vue')
        },
        {
            path: '/create',
            name: 'create',
            meta: { authorize: true },
            component: () => import('../views/CreateIdcardView.vue')
        }   
    ]
});

router.beforeEach((to, from, next) => {
    const authenticated = store.state.user.isLoggedIn;
    if (to.meta.authorize && !authenticated) {
        next("/login");
        return;
    }
    next();
    return;
});

export default router;
