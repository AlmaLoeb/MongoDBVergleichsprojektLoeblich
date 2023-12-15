<template>
  <div class="loginForm">
    <template v-if="!isUserLoggedIn">
      <label>Username: <input type="text" v-model="username" /></label>
      <label>Password: <input type="password" v-model="password" /></label>
      <p class="error" v-if="errorMessage">{{ errorMessage }}</p>
      <button type="button" @click="login">Submit</button>
    </template>
    <template v-else>
      Logged in as {{ username }}
      <a href="javascript:void(0)" @click="logout">Logout</a>
    </template>
  </div>
</template>

<script>
import { ref, computed, watchEffect } from 'vue';
import axios from 'axios';
import { useStore } from 'vuex';

export default {
  setup() {
    const store = useStore();
    const username = ref('');
    const password = ref('');
    const errorMessage = ref('');

    const isUserLoggedIn = computed(() => store.state.user.isLoggedIn);

    const login = async () => {
      errorMessage.value = '';
      try {
        const response = await axios.post('user/login', {
          username: username.value,
          password: password.value,
        });

        axios.defaults.headers.common['Authorization'] = `Bearer ${response.data.Token}`;

        store.commit('authenticate', {
          username: response.data.Username,
          token: response.data.Token,
        });

        username.value = '';
        password.value = '';

      } catch (e) {
        if (e.response.status == 401) {
          errorMessage.value = 'Login failed. Invalid credentials. Try again!';
        }
      }
    };

    const logout = () => {
      delete axios.defaults.headers.common['Authorization'];
      store.commit('authenticate', null);
    };

    watchEffect(() => {
      if (username.value || password.value) {
        errorMessage.value = '';
      }
    })

    return {
      username,
      password,
      login,
      logout,
      isUserLoggedIn,
      errorMessage,
    };
  },
};
</script>

<style scoped>
.loginForm {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  max-width: 300px;
  margin: 0 auto;
  border: 3px solid hsl(234, 100%, 96%);
  padding: 1rem;
  border-radius: 0.5rem;
  margin-top: 2rem;
}
a {
  text-decoration: none;
}
a:hover {
  color: lightgray;
  text-decoration: underline;
}
button {
  background-color: hsl(234, 100%, 96%);
  border: 0.25px solid #000000;
  padding: 0.5rem 1rem;
}
</style>
