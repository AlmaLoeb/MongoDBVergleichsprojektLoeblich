<template>
  <div v-if="show" class="edit-dialog">
    <h2>Edit Password</h2>
    <form @submit="updatePassword">
      <p>Website URL:</p>
      <input
        v-model="localDetails.websiteUrl"
        placeholder="Website URL"
        required
      />
      <div v-if="validation.websiteUrl" class="error">
        {{ validation.websiteUrl }}
      </div>

      <p>Account Name:</p>
      <input
        v-model="localDetails.accountname"
        placeholder="Account Name"
        required
      />
      <div v-if="validation.accountname" class="error">
        {{ validation.accountname }}
      </div>

<p>Password:</p>
<input  v-model="localDetails.passworde" placeholder="Password" required />
<div v-if="validation.passworde" class="error">{{ validation.passworde }}</div>

        <button class="submit" type="submit">Save Changes</button>
      </form>
      <button @click="closeDialog">Close</button>
    </div>
  </template>
  
  <script>
  import axios from "axios";
  
  export default {
    props: {
      show: Boolean,
      details: Object,
    },
    data() {
    return {
      localDetails: null,
      validation: {},
    };
  },

  watch: {
    details(newDetails) {
      this.localDetails = { ...newDetails };
    },
  },
  methods: {
    async updatePassword(e) {
      e.preventDefault();

      const updateData = {
        guid: this.localDetails.guid,
        id: this.localDetails.id,
        websiteUrl: this.localDetails.websiteUrl,
        accountname: this.localDetails.accountname,
        passwordPoliciesGuid: this.localDetails.passwordPoliciesGuid,
        passworde: this.localDetails.passworde,
      };

      try {
        const response = await axios.put(
          `password/${updateData.guid}`,
          updateData
        );
        console.log(response.data);
        this.$emit("password-updated", response.data);

        this.validation = {};
      } catch (error) {
        if (error.response && error.response.data.errors) {
          this.validation = Object.keys(error.response.data.errors).reduce(
            (prev, key) => {
              const newKey = key.charAt(0).toLowerCase() + key.slice(1);
              prev[newKey] = error.response.data.errors[key][0];
              return prev;
            },
            {}
          );
        }
      }
    },

    closeDialog() {
      this.$emit("close");
    },
  },
};
</script>
