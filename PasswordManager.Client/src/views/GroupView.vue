<template>
  <div class="groupView">
    <h1>Group {{ groupId }}</h1>
    <div class="passwordEntries">
      <h2>Passwords</h2>
      <table class="passwordTable">
        <thead>
          <tr>
            <th>Website URL</th>
            <th>Account Name</th>
            <th>Password</th>

            <th>Length</th>
            <th>Safeness</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="p in passwords" :key="p.guid">
            <td>{{ p.websiteUrl }}</td>
            <td>{{ p.accountname }}</td>
            <td>{{ p.passworde }}</td>
            <td>{{ p.length }}</td>
            <td>{{ p.safeness }}</td>
            <td>
              <button @click="editPassword(p)">Edit</button>
              <button
                @click="
                  showDeleteDialog = true;
                  selectedPasswordGuid = p.guid;
                "
              >
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <!-- Delete Dialog -->
    <div v-if="showDeleteDialog" class="delete-dialog">
      <h2>Confirm Delete</h2>
      <p>Are you sure you want to delete this password?</p>
      <button @click="deletePassword">Yes</button>
      <button @click="showDeleteDialog = false">No</button>
    </div>
    <EditPassword
      :show="showEditPasswordDialog"
      :details="editPasswordDetails"
      @password-updated="passwordUpdated"
      @close="onPasswordEditClose"
    />

    <div class="bankcardEntries">
      <h2>Bankcards</h2>
      <table class="bankcardTable">
        <thead>
          <tr>
            <th>Card Number</th>
            <th>Cardholder</th>
            <th>Cardholder Firstname</th>
            <th>Expiry Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="b in bankcards" :key="b.iban">
            <td>{{ b.cardnumber }}</td>
            <td>{{ b.cardholder }}</td>
            <td>{{ b.carholderfirst }}</td>
            <td>{{ b.expirydate }}</td>
            <td>
              <button @click="editBankcard(b)">Edit</button>
              <button
                @click="
                  showDeleteBankcardDialog = true;
                  selectedBankcardId = b.iban;
                "
              >
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <EditBankcard
      :show="showEditBankcardDialog"
      :details="editBankcardDetails"
      @bankcard-updated="bankcardUpdated"
      @close="onBankcardEditClose"
      @refresh-data="fetchData"
    />

    <!-- Delete Bankcard Dialog -->
    <div v-if="showDeleteBankcardDialog" class="delete-dialog">
      <h2>Confirm Delete</h2>
      <p>Are you sure you want to delete this bankcard?</p>
      <button @click="deleteBankcard">Yes</button>
      <button @click="showDeleteBankcardDialog = false">No</button>
    </div>

    <div class="idcardEntries">
      <h2>ID Cards</h2>
      <table class="idcardTable">
        <thead>
          <tr>
            <th>Card Number</th>
            <th>Cardholder</th>
            <th>Cardholder firstname</th>
            <th>Expiry Date</th>
            <th>Birthdate</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="i in idcards" :key="i.idcardnr">
            <td>{{ i.idcardnr }}</td>
            <td>{{ i.surname }}</td>
            <td>{{ i.firstname }}</td>
            <td>{{ i.expirationdate }}</td>
            <td>{{ i.birthdate }}</td>
            <td>
              <button @click="editIdcard(i)">Edit</button>
              <button
                @click="
                  showDeleteIdcardDialog = true;
                  selectedIdcardGuid = i.idcardnr;
                "
              >
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <EditIdcard
      :show="showEditIdcardDialog"
      :details="editIdcardDetails"
      @idcard-updated="idcardUpdated"
      @close="onIdcardEditClose"
    />

    <div v-if="showDeleteIdcardDialog" class="delete-dialog">
      <h2>Confirm Delete</h2>
      <p>Are you sure you want to delete this ID card?</p>
      <button @click="deleteIdcard">Yes</button>
      <button @click="showDeleteIdcardDialog = false">No</button>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import EditPassword from "../components/EditPassword.vue";
import EditIdcard from "../components/EditIdcard.vue";
import EditBankcard from "../components/EditBankcard.vue";

export default {
  components: {
    EditBankcard,
    EditPassword,
    EditIdcard,
  },
  data() {
    return {
      passwords: [],
      bankcards: [],
      idcards: [],
      showDeleteDialog: false,
      selectedPasswordGuid: null,
      showDeleteIdcardDialog: false,
      selectedIdcardGuid: null,
      showEditDialog: false,
      selectedPassword: null,
      editPasswordDetails: null,
      editIdcardDetails: null,
      showEditIdcardDialog: false,
      showEditPasswordDialog: false,
      showEditBankcardDialog: false,
      editBankcardDetails: null,
      groupName: "",
      showDeleteBankcardDialog: false,
      selectedBankcardId: null,
    };
  },

  async mounted() {
    await this.fetchData();
  },
  computed: {
    groupId() {
      return this.$route.params.id;
    },
  },
  methods: {
    async fetchData() {
      const response = await axios.get(`group/${this.groupId}/passwords`);

      this.passwords = response.data.passwords.map((p) => ({
        ...p,
        websiteUrl: p.websiteUrl,
        accountname: p.accountname,
        passworde: p.passworde,
        length: p.length,
        safeness: p.safeness,
      }));

      this.bankcards = response.data.bankcards.map((b) => ({
        ...b,
        cardnumber: b.iban,
        cardholder: b.surname,
        carholderfirst: b.firstname,
        expirydate: b.expirationdate,
      }));

      this.idcards = response.data.idcards.map((i) => ({
        ...i,
        cardnumber: i.idcardnr,
        cardholder: i.surname,
        firstname: i.firstname,
        expirydate: i.expirationdate,
        birthdate: i.birthdate,
      }));
    },
    closeEditDialog() {
      this.showEditPasswordDialog = false;
    },
    passwordUpdated(updatedPassword) {
      try {
        const index = this.passwords.findIndex(
          (p) => p.guid === updatedPassword.password.guid
        );
        if (index !== -1) {
          this.passwords.splice(index, 1, {
            ...updatedPassword.password,
            length: updatedPassword.length,
            safeness: updatedPassword.safeness,
          });
        } else {
          console.error(
            "Updated password GUID not found in password array:",
            updatedPassword
          );
        }
      } catch (error) {
        console.error("Error in passwordUpdated method:", error);
      }
    },

    onPasswordEditClose(updatedPassword) {
      if (updatedPassword) {
        const updatedIndex = this.passwords.findIndex(
          (p) => p.guid === updatedPassword.guid
        );
        if (updatedIndex !== -1) {
          this.passwords.splice(updatedIndex, 1, updatedPassword);
        }
      }
      this.showEditPasswordDialog = false;
    },

    editPassword(password) {
      this.editPasswordDetails = { ...password };
      this.showEditPasswordDialog = true;
    },
    idcardUpdated(updatedIdcard) {
      const index = this.idcards.findIndex(
        (i) => i.idcardnr === updatedIdcard.idcardnr
      );
      if (index !== -1) {
        this.idcards.splice(index, 1, updatedIdcard);
      } else {
        console.error(
          "Updated ID card ID not found in idcards array:",
          updatedIdcard
        );
      }
    },

    onIdcardEditClose() {
      this.showEditIdcardDialog = false;
    },

    editIdcard(idcard) {
      console.log("edited");
      this.editIdcardDetails = { ...idcard };
      this.showEditIdcardDialog = true;
    },
    async deleteBankcard() {
      try {
        await axios.delete(`/bankcard/${this.selectedBankcardId}`);

        this.bankcards = this.bankcards.filter(
          (b) => b.iban !== this.selectedBankcardId
        );
        this.showDeleteBankcardDialog = false;
      } catch (error) {
        alert("Failed to delete bankcard");
      }
    },

    async deletePassword() {
      try {
        await axios.delete(`password/${this.selectedPasswordGuid}`);

        this.passwords = this.passwords.filter(
          (p) => p.guid !== this.selectedPasswordGuid
        );
        this.showDeleteDialog = false;
      } catch (error) {
        alert("Failed to delete password");
      }
    },
    editBankcard(bankcard) {
      this.editBankcardDetails = { ...bankcard };
      this.showEditBankcardDialog = true;
    },
    onBankcardEditClose(updatedBankcard) {
      if (updatedBankcard) {
        const updatedIndex = this.bankcards.findIndex(
          (b) => b.id === updatedBankcard.id
        );
        if (updatedIndex !== -1) {
          this.bankcards.splice(updatedIndex, 1, updatedBankcard);
        }
      }
      this.showEditBankcardDialog = false;
    },
    bankcardUpdated(updatedBankcard) {
      const index = this.bankcards.findIndex(
        (b) => b.iban === updatedBankcard.iban
      );

      if (index !== -1) {
        this.bankcards.splice(index, 1, updatedBankcard);
      } else {
        console.error(
          "Updated bankcard ID not found in bankcards array:",
          updatedBankcard
        );
      }
    },

    async deleteIdcard() {
      try {
        await axios.delete(`/idcard/${this.selectedIdcardGuid}`);

        this.idcards = this.idcards.filter(
          (i) => i.idcardnr !== this.selectedIdcardGuid
        );
        this.showDeleteIdcardDialog = false;
      } catch (error) {
        alert("Failed to delete ID card");
      }
    },
  },
};
</script>

<style scoped>
.groupView {
  text-align: center;
  justify-content: center;
  align-items: center;
  margin-bottom: 2rem;
}
.passwordTable,
.bankcardTable,
.idcardTable {
  width: 80%;
  align-content: center;
  border-collapse: collapse;
  margin-bottom: 20px;
  justify-content: center;
  margin: 0 auto;
}

.passwordTable th,
.bankcardTable th,
.idcardTable th {
  background-color: hsl(234, 100%, 96%);
  padding: 10px;
  font-weight: bold;
  text-align: center;
}

.passwordTable td,
.bankcardTable td,
.idcardTable td {
  padding: 10px;
  border-bottom: 1px solid hsl(234, 100%, 96%);
}

.passwordTable button,
.bankcardTable button,
.idcardTable button {
  margin-right: 5px;
  background-color: hsl(234, 100%, 96%);
  border: 0.5px solid #000000;
}
.submit {
  margin-left: 10px;
}
.actions {
  display: flex;
  justify-content: center;
}

.delete-dialog {
  padding: 1rem;
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 9999;
  border: 0.5px solid #000000;
  background-color: hsl(234, 100%, 96%);
}

.edit-dialog {
  border: 0.5px solid #000000;
  background-color: hsl(234, 100%, 96%);
  padding: 1rem;
  position: fixed;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  z-index: 1000;
  width: 25vw;
}
</style>
