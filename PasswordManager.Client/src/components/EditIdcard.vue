<template>
  <div v-if="show" class="edit-dialog">
    <h2>Edit ID Card</h2>
    <form @submit="updateIdcard">
      <p>Cardholder:</p>
      <input
        v-model="localDetails.cardholder"
        placeholder="Cardholder Name"
        required
      />

      <p>Firstname:</p>
      <input
        v-model="localDetails.firstname"
        placeholder="Cardholder Firstname"
        required
      />

      <p>Expiration Date:</p>
      <input
   
        v-model="localDetails.expirationdate"
        placeholder="Expiration Date"
        required
      />

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
    };
  },

  watch: {
    details(newDetails) {
      this.localDetails = { ...newDetails };
    },
  },
  methods: {
    async updateIdcard(e) {
      e.preventDefault();

      const updateData = {
        idcardnr: this.localDetails.idcardnr,
        firstname: this.localDetails.firstname,
        surname: this.localDetails.cardholder,
        expirationdate: this.localDetails.expirationdate,
        birthdate: this.localDetails.birthdate,
      };

      try {
        const response = await axios.put(
          `idcard/${updateData.idcardnr}`,
          updateData
        );
        this.$emit("idcard-updated", response.data);
      } catch (error) {}
    },
    closeDialog() {
      this.$emit("close");
    },
  },
};
</script>
