<template>
  <div class="groupDisplay">
 
  
      <h4>
        {{ password.name }}
      </h4>

  </div>
</template>

<style scoped>
.groupDisplay {
  width: 600px;
  height: 50px;
  border: 1px solid black;
  border-radius: 5px;
  display: flex;
  flex-direction: column;
  padding: 0.5rem;
  text-decoration: none;
  align-items: center;  
  justify-content: center;  
}

h4 {
  color: black;
  text-align: center;
  text-decoration: none;
}

</style>

<script>


export default {
  props: {
    password: Object,
  },
 
};
</script>
