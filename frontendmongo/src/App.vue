<template>
  <div id="app" class="password-manager-app">
    <div class="header">
      <img src="./components/keanuportrait.png" alt="Keanu Image" class="left-image">
      <router-link to="/" class="password-manager-heading">Password Manager</router-link>
      <img src="./components/keanuportrait.png" alt="Keanu Image" class="right-image">
    </div>
    <router-view />
  </div>
</template>

<script>
export default {
  name: 'App',
};
</script>

<style scoped>
/* Global styles */
.password-manager-app {
  text-align: center;
  padding: 20px;
}

.header {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 20px;
}

.password-manager-heading {
  font-size: 32px;
  color: #007bff; /* Blue color for the heading */
  margin: 0 10px; /* Add margin to space the heading from the images */
  text-decoration: none; /* Remove the default link underline */
}

.left-image,
.right-image {
  width: 20px; /* Adjust the width of the images as needed */
  height: auto;
}
</style>
