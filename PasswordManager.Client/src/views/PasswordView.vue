<script setup>
import axios from "axios"
import SpinnerComponent from '../components/SpinnerComponent.vue';
</script>

<template>
    <div class="passwordView">
        <SpinnerComponent v-if="loading"></SpinnerComponent>
        <h1>Password</h1>
        <div v-if="password" class="passwordDetail">
            <table>
                <tr>
                    <td>Website URL:</td>
                    <td>{{ password.websiteUrl }}</td>
                </tr>
                <tr>
                    <td>Account Name:</td>
                    <td>{{ password.accountname }}</td>
                </tr>
                <tr>
                    <td>Password:</td>
                    <td>{{ password.passworde }}</td>
                </tr>
                <tr>
                    <td>Password Policies Guid:</td>
                    <td>{{ password.passwordPoliciesGuid }}</td>
                </tr>
                <tr>
                    <td>Password Length:</td>
                    <td>{{ password.length }}</td>
                </tr>
                <tr>
                    <td>Password Safeness:</td>
                    <td>{{ password.safeness }}</td>
                </tr>
            </table>
        </div>
    </div>
</template>

<style scoped>
.passwordDetail {
    border: 1px solid gray;
    padding: 1em;
    color: black; 
}

.passwordDetail:hover {
    background-color: var(--main-color-light);
}

.passwordDetail table {
    width: 100%;
    table-layout: fixed;
}

.passwordDetail td:first-child {
    width: 40%;
    font-weight: bold;
}
</style>

<script>
export default {
    data() {
        return {
            password: null,
            loading: false
        }
    },
    async mounted() {
        try {
            this.loading = true;
            const response = await axios.get(`password/${this.passwordGuid}`);
            this.password = response.data;
        }
        catch (e) {
            alert("Server is not reachable.");
        }
        finally {
            this.loading = false;
        }
    },
    computed: {
        passwordGuid() { return this.$route.params.id; }
    }
}
</script>
