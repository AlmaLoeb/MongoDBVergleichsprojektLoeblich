<template>
  <div v-if="show" class="edit-dialog">
    <h2>Edit Bank Card</h2>
    <form @submit="updateBankCard">
      <p>First Name:</p>
      <input
        v-model="localDetails.firstname"
        placeholder="First Name"
        required
      />
      <div v-if="validation.firstname" class="error">
        {{ validation.firstname }}
      </div>

      <p>Surname:</p>
      <input v-model="localDetails.surname" placeholder="Surname" required />
      <div v-if="validation.surname" class="error">
        {{ validation.surname }}
      </div>

      <p>Expiration Date:</p>
      <input
        v-model="localDetails.expirationdate"
        placeholder="Expiration Date"
        required
      />
      <div v-if="validation.expirationdate" class="error">
        {{ validation.expirationdate }}
      </div>

      <button type="submit">Save Changes</button>
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
    async updateBankCard(e) {
      e.preventDefault();
      const updateData = {
        iban: this.localDetails.iban,
        firstname: this.localDetails.firstname,
        surname: this.localDetails.surname,
        expirationdate: this.localDetails.expirationdate,
      };

      try {
        await axios.put(`bankcard/${updateData.iban}`, updateData);

        // refreshing the data
        this.$emit("refresh-data");
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
