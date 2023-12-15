<template>
  <div>
    <h2 class="my-4">Bankcards</h2>

    <!-- Filter and Sort Options -->
    <div class="mb-3">
      <label for="filterIban" class="form-label">Filter by IBAN:</label>
      <input v-model="filterIban" type="text" class="form-control" id="filterIban" placeholder="Enter IBAN">
    </div>
  <div class="mb-3">
    <label for="sortLastName" class="form-label">Sort by Last Name:</label>
    <div class="d-flex align-items-center">
      <select v-model="sortOptionLastName" class="form-select" id="sortLastName">
        <option value="asc">Ascending &#9650;</option>
        <option value="desc">Descending &#9660;</option>
      </select>
    </div>
  </div>

    <!-- Bankcard List Table -->
    <div class="table-container">
      <table class="table table-striped">
        <thead>
          <tr>
            <th>IBAN</th>
            <th>First Name</th>
            <th>Surname</th>
            <th>Expiration Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="bankcard in sortedBankcards" :key="bankcard.id">
            <td>{{ bankcard.iban }}</td>
            <td>{{ bankcard.firstname }}</td>
            <td>{{ bankcard.surname }}</td>
            <td>{{ bankcard.expirationDate }}</td>
            <td>
              <button class="btn btn-primary btn-sm me-2" @click="openEditModal(bankcard)">Edit</button>
              <button class="btn btn-danger btn-sm" @click="confirmDelete(bankcard.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Add Bankcard Form -->
    <div class="mb-3">
      <button class="btn btn-success" @click="openAddModal()">Add Bankcard</button>
    </div>
    <!-- The Edit Modal -->
    <div class="modal" v-if="editModalOpen">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-primary text-white">
            <h5 class="modal-title">Edit Bankcard</h5>
            <button type="button" class="btn-close" @click="editModalOpen = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label for="iban" class="form-label">IBAN</label>
              <input v-model="editFormData.iban" type="text" class="form-control" id="iban" placeholder="IBAN">
            </div>
            <div class="mb-3">
              <label for="firstName" class="form-label">First Name</label>
              <input v-model="editFormData.firstname" type="text" class="form-control" id="firstName" placeholder="First Name">
            </div>
            <div class="mb-3">
              <label for="surname" class="form-label">Surname</label>
              <input v-model="editFormData.surname" type="text" class="form-control" id="surname" placeholder="Surname">
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-primary" @click="submitEdit()">Submit</button>
            <button class="btn btn-secondary" @click="editModalOpen = false">Cancel</button>
          </div>
        </div>
      </div>
    </div>
      <!-- Add Modal -->
      <div class="modal" v-if="addModalOpen">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-success text-white">
            <h5 class="modal-title">Add Bankcard</h5>
            <button type="button" class="btn-close" @click="addModalOpen = false"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label for="iban" class="form-label">IBAN</label>
              <input v-model="addFormData.iban" type="text" class="form-control" id="iban" placeholder="IBAN">
            </div>
            <div class="mb-3">
              <label for="firstName" class="form-label">First Name</label>
              <input v-model="addFormData.firstname" type="text" class="form-control" id="firstName" placeholder="First Name">
            </div>
            <div class="mb-3">
              <label for="surname" class="form-label">Surname</label>
              <input v-model="addFormData.surname" type="text" class="form-control" id="surname" placeholder="Surname">
            </div>
            <div class="mb-3">
              <label for="expirationDate" class="form-label">Expiration Date</label>
              <input v-model="addFormData.expirationDate" type="text" class="form-control" id="expirationDate" placeholder="Expiration Date">
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-success" @click="submitAdd()">Add</button>
            <button class="btn btn-secondary" @click="addModalOpen = false">Cancel</button>
          </div>
        </div>
      </div>
    </div>
    <img src="./keanu.png" alt="Keanu Image" class="keanu-image">
  </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';

export default {
  name: 'GroupDetailComponent',
  props: {
    groupName: String
  },
  setup(props) {
    const route = useRoute();
    const groupId = route.params.groupId;
    const bankcards = ref([]);
    const editModalOpen = ref(false);
    const editFormData = ref({});
    const addModalOpen = ref(false);
    const addFormData = ref({
      iban: '',
      firstname: '',
      surname: '',
      expirationDate: ''
    });
    const filterIban = ref('');
    const sortOption = ref('asc');
    const sortOptionLastName = ref('asc'); // Add a ref for sorting option by last name

    onMounted(async () => {
      try {
        const response = await axios.get(`/api/mongo/${groupId}/bankcards`);
        bankcards.value = response.data;
      } catch (error) {
        console.error('Error fetching bankcards:', error);
      }
    });

    const filteredBankcards = computed(() => {
      return bankcards.value.filter((bankcard) => {
        return bankcard.iban.toLowerCase().includes(filterIban.value.toLowerCase());
      });
    });

    const sortedBankcards = computed(() => {
      const sorted = [...filteredBankcards.value];
      if (sortOptionLastName.value === 'asc') {
        sorted.sort((a, b) => a.surname.localeCompare(b.surname));
      } else if (sortOptionLastName.value === 'desc') {
        sorted.sort((a, b) => b.surname.localeCompare(a.surname));
      }
      return sorted;
    });
    const openEditModal = (bankcard) => {
      editFormData.value = { ...bankcard };
      editModalOpen.value = true;
    };

    const submitEdit = async () => {
  if (!editFormData.value.id) {
    console.error('Bankcard ID is undefined');
    return;
  }

  try {
    console.log('PUT Request URL:');
    console.log(`/api/mongo/${groupId}/bankcards/${editFormData.value.id}`);

    console.log('PUT Request Body:');
    console.log({
      ...editFormData.value
    });

    const response = await axios.put(`/api/mongo/${groupId}/bankcards/${editFormData.value.id}`, {
      ...editFormData.value
    });

    console.log('PUT Response:');
    console.log(response.data);

    // Update local bankcards list
    const index = bankcards.value.findIndex((b) => b.id === editFormData.value.id);
    if (index !== -1) {
      bankcards.value[index] = { ...editFormData.value };
    }
    editModalOpen.value = false;
  } catch (error) {
    console.error('Error updating bankcard:', error);
  }
};
const openAddModal = () => {
      addFormData.value = {
        iban: '',
        firstname: '',
        surname: '',
        expirationDate: ''
      };
      addModalOpen.value = true;
    };
    const submitAdd = async () => {
  try {
    // Log the request data before sending
    console.log('Request Data:', {
      ...addFormData.value
    });

    const response = await axios.post(`/api/mongo/${groupId}/bankcards`, {
      ...addFormData.value
    });

    bankcards.value.push(response.data);
    addModalOpen.value = false;
  } catch (error) {
    console.error('Error adding bankcard:', error);
    if (error.response) {
      console.error('Response Data:', error.response.data);
    }
  }
};



    const confirmDelete = async (bankcardIdStr) => {
      if (window.confirm("Are you sure you want to delete this bankcard?")) {
        try {
          await axios.delete(`/api/mongo/${groupId}/bankcards/${bankcardIdStr}`);
          // Remove from local bankcards list
          bankcards.value = bankcards.value.filter((b) => b.id !== bankcardIdStr);
        } catch (error) {
          console.error('Error deleting bankcard:', error);
        }
      }
    };
    return {
      groupName: props.groupName,
      bankcards,
      editModalOpen,
      editFormData,
      openEditModal,
      submitEdit,
      confirmDelete,
      addModalOpen,
      addFormData,
      openAddModal,
      submitAdd,
      filterIban, 
      sortedBankcards, 
      sortOption,
      sortOptionLastName
    };
  },
};
</script>


<style>
/* Additional Styles for your bankcard list component */
/* Customize these styles as needed */
.table {
  font-size: 16px;
}

.btn-primary {
  background-color: #007bff;
}

.btn-danger {
  background-color: #dc3545;
}

.modal-content {
  border-radius: 10px;
}

.modal-header {
  border-radius: 10px 10px 0 0;
}
</style>
<style scoped>
/* Additional Styles for your bankcard list component */
/* Customize these styles as needed */
.table-container {
  display: flex;
  justify-content: center;
}

.table {
  font-size: 16px;
  border-collapse: collapse; /* Add this to collapse the borders */
  width: 100%;
  margin-bottom: 20px; /* Add some spacing at the bottom of the table */
}

.table th,
.table td {
  border: 1px solid #ddd; /* Add a 1px solid border */
  padding: 8px;
  text-align: left;
}

.table th {
  background-color: #f2f2f2; /* Add a background color for header cells */
}

.btn-primary {
  background-color: #007bff;
}

.btn-danger {
  background-color: #dc3545;
}

.modal-content {
  border-radius: 10px;
}
</style>
