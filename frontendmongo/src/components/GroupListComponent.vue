<template>
  <div class="group-list-container">
    <h2>Groups</h2>
    <div v-if="loading" class="loading-text">Loading groups...</div>
    <ul v-else>
      <li v-for="group in groups" :key="group.id">
        <router-link :to="{ name: 'group-detail', params: { groupId: group.id } }">
          {{ group.name }}
        </router-link>
      </li>
    </ul>
    <!-- Responsive image -->
    <img src="./keanu.png" alt="Keanu Image" class="keanu-image">
  </div>
</template>

<script>
import axios from 'axios';
import { ref, onMounted } from 'vue';

export default {
  name: 'GroupListComponent',
  setup() {
    const groups = ref([]);
    const loading = ref(true);

    onMounted(async () => {
      loading.value = true;
      try {
        const response = await axios.get('/api/mongo');
        // Convert each group's ID to a string here
        groups.value = response.data.map(group => ({
          ...group,
          id: group.id.toString() // Assuming 'id' exists directly on the group object and is an ObjectId
        }));
      } catch (error) {
        console.error('Error fetching groups:', error);
      } finally {
        loading.value = false;
      }
    });

    return {
      groups,
      loading
    };
  }
};
</script>

<style scoped>
/* Styles for your group list component */
.group-list-container {
  text-align: center;
  padding: 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.loading-text {
  font-weight: bold;
  color: #007bff; /* Blue color for loading text */
}

ul {
  list-style-type: none;
  padding: 0;
  margin: 0; /* Remove default margin for the ul element */
}

li {
  margin: 10px 0;
  font-size: 18px;
}

/* Responsive image styles */
.keanu-image {
  max-width: 100%; /* Ensure the image does not exceed its parent's width */
  height: auto; /* Maintain the image's aspect ratio */
  margin-top: auto; /* Push the image to the bottom of the container */
}
</style>
