<script setup>
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div class="appContainer">
    <nav>
      <RouterLink to="/">
        <div class="navitem">
          <div class="label">Home</div>
        </div>
      </RouterLink>
      <RouterLink to="/create">
        <div class="navitem">
          <div class="label">Add Id Card</div>
        </div>        
      </RouterLink>
    </nav>
    <main>
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
.appContainer {
  display: flex;
  flex-direction: column;
  width: 100%;
}

nav {
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  background-color: var(--main-color-light);
  padding: 1rem 0;
}

nav a {
  color: var(--main-color);
  text-decoration: none;
}

.navitem {
  display: flex;
  flex-direction: column;
  align-items: center;
}



.navitem:hover {
  color: var(--main-color-dark);
}

main {
  flex-grow: 1;
  padding: 1rem;
  overflow-y: auto;
  
}
</style>
