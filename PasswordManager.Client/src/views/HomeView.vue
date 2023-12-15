<script setup>
import axios from 'axios'
import GroupDisplay from '../components/GroupDisplay.vue'
import SpinnerComponent from '../components/SpinnerComponent.vue'
</script>

<template>
  <div class= "homeContainer">
    <SpinnerComponent v-if="loading"></SpinnerComponent>
    <h1>Home</h1>
    <h2>{{ groups.length }} groups</h2>
    <div class="groupDisplays">
      <div v-for="p in groups" v-bind:key="p.id" class="passwordRow">
        <RouterLink v-bind:to="`/group/${p.id}`">
          <GroupDisplay v-bind:password="p" ></GroupDisplay>
        </RouterLink>
        <button @click="showDeleteDialog = true; selectedGroup = p.id" class="deleteButton">Delete</button> <!-- Delete button -->
      </div>

      <!-- Delete Dialog -->
      <div v-if="showDeleteDialog" class="delete-dialog">
        <h2>Confirm Delete</h2>
        <p>Are you sure you want to delete this group?</p>
        <button @click="deleteGroup">Yes</button>
        <button @click="showDeleteDialog = false">No</button>
      </div>
      
    </div>
  </div>
</template>

<style scoped>
button{
  margin-left: 20px;
}
a{
  text-decoration: none ;
}
.groupDisplays{
  display: grid;
  gap: 1rem;
  justify-items: center;
}

.passwordRow {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  
}

.deleteButton
{
  background-color: hsl(234, 100%, 96%);
  height:70px;
  border-radius: 5px;
  border: 1px solid black;
}

.homeContainer
{
  text-align: center;
}

.delete-dialog {
  border: 0.5px solid #000000;
  background-color:  hsl(234, 100%, 96%);
  padding: 1rem;
  position: fixed;

}
</style>

<script>
export default {
  data() {
    return {
      groups: [],
      loading: false,
      showDeleteDialog: false,
      selectedGroup: null
    };
  },
  async mounted() {
    try{
      this.loading = true;
      this.groups= (await axios.get("group")).data;
    }
    catch (e) {
      alert("Server not reachable.");
    }
    finally {
      this.loading = false;      
    }
  },
  methods: {
    async deleteGroup() {
        try {
            await axios.delete(`group/${this.selectedGroup}`);
            this.groups = this.groups.filter(g => g.id !== this.selectedGroup);
            this.showDeleteDialog = false;
        } catch(e) {
            alert('Failed to delete group');
        }
    }
  }
}
</script>
