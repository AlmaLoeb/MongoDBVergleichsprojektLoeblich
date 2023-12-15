<script setup>
import axios from "axios";
</script>
<template>
  <div class="createIdcard" id="createIdcard">
    <h3>New Idcard</h3>
    <div class="form">
      <div class="form-row">
        <div class="label">Group Id</div>
        <div class="control">
          <select v-model="model.groupid">
            <option v-for="group in groups" :value="group.id">
              {{ group.name }}
            </option>
          </select>
          <div v-if="validation.groupId" class="error">
            {{ validation.groupId }}
          </div>
        </div>
      </div>
      <div class="form-row">
        <div class="label">Id card Number</div>
        <div class="control">
          <input type="text" v-model="model.idcardnr" placeholder="12345" />
          <div v-if="validation.idcardnr" class="error">
            {{ validation.idcardnr }}
          </div>
        </div>
      </div>
      <div class="form-row">
        <div class="label">Surname</div>
        <div class="control">
          <input type="text" v-model="model.surname" placeholder="Mustermann" />
          <div v-if="validation.surname" class="error">
            {{ validation.surname }}
          </div>
        </div>
      </div>
      <div class="form-row">
        <div class="label">Firstname</div>
        <div class="control">
          <input type="text" v-model="model.firstname" placeholder="Max" />
          <div v-if="validation.firstname" class="error">
            {{ validation.firstname }}
          </div>
        </div>
      </div>
      <div class="form-row">
        <div class="label">Expiration date</div>
        <div class="control">
          <input type="datetime-local" v-model="model.expirationdate" />
          <div v-if="validation.expirationdate" class="error">
            {{ validation.expirationdate }}
          </div>
        </div>
      </div>
      <div class="form-row">
        <div class="label">Birthdate</div>
        <div class="control">
          <input type="datetime-local" v-model="model.birthdate" />
          <div v-if="validation.birthdate" class="error">
            {{ validation.birthdate }}
          </div>
        </div>
      </div>
      <div class="form-row">
        <div class="label"></div>
        <div class="control">
          <button type="submit" v-on:click="sendData()">Add</button>
          <br>
          <div v-if="message">{{ message }}</div>
        </div>
      </div>
    </div>
  </div>
</template>
<style scoped>
.createIdcard {
  gap: 1rem;
  text-align: center;
  width: 100%;
  max-width: 500px;
  margin: auto;
  border: 3px solid hsl(234, 100%, 96%);
  padding: 1rem;
  border-radius: 0.5rem;
  margin-top: 2rem;
}
.form-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}
.label {
  flex: 1;
}
.control {
  flex: 2;
  display: flex; 
  justify-content: center; 
}
.control > input,
.control > select {
  width: 100%;
  padding: 0.5rem;
  box-sizing: border-box;
}
button {
  background-color: hsl(234, 100%, 96%);
  border: 0.5px solid #000000;
  padding: 0.5rem 1rem;
}
</style>

<script>
export default {
  data() {
    return {
      model: {},
      groups: [],

      loading: false,
      validation: {},
      message: "",
    };
  },
  async mounted() {
    try {
      this.loading = true;
      this.groups = (await axios.get("group")).data;
    } catch (e) {
      alert("Server not reachable.");
    } finally {
      this.loading = false;
    }
  },
  methods: {
    async sendData() {
      this.validation = {};
      try {
        await axios.post("Idcard", this.model);
        this.model = {};
        this.validation = {};
        this.message = "Your Idcard has been saved.";
        setTimeout(() => {
          this.message = "";
        }, 20000);
      } catch (e) {
        if (e.response.status == 400) {
          this.validation = Object.keys(e.response.data.errors).reduce(
            (prev, key) => {
              const newKey = key.charAt(0).toLowerCase() + key.slice(1);
              prev[newKey] = e.response.data.errors[key][0];
              return prev;
            },
            {}
          );
          console.log(this.validation);
        }
      }
    },
  },
};
</script>
