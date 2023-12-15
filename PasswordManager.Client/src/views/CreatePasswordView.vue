<script setup>
import axios from 'axios';
</script>

<template>
      
    <div class="createPassword" id="createPassword">
        <h3>New Password</h3>
        <div class="form">
            <div class="form-row">
                <div class="label">Group Id</div>
                <div class="control">
                    <input type="text" v-model="model.groupid" />
                    <div v-if="validation.groupid" class="error">{{ validation.groupid }}</div>
                </div>
            </div>
            <div class="form-row">
                <div class="label">Account Name</div>
                <div class="control">
                    <input type="text" v-model="model.accountname" />
                    <div v-if="validation.accountname" class="error">{{ validation.accountname }}</div>
                </div>
            </div>
            <div class="form-row">
                <div class="label">Website URL</div>
                <div class="control">
                    <input type="text" v-model="model.websiteUrl" />
                    <div v-if="validation.websiteUrl" class="error">{{ validation.websiteUrl }}</div>
                </div>
            </div>
            <div class="form-row">
                <div class="label">Password</div>
                <div class="control">
                    <input type="password" v-model="model.passworde" />
                    <div v-if="validation.passworde" class="error">{{ validation.passworde }}</div>
                </div>
            </div>
         <!--   <div class="form-row">
                <div class="label">Password Policy</div>
                <div class="control">
                    <input type="text" v-model="model.passwordPoliciesGuid" />
                    <div v-if="validation.passwordPoliciesGuid" class="error">{{ validation.passwordPoliciesGuid }}</div>
                </div>
            </div>-->
            <div class="form-row">
                <div class="label"></div>
                <div class="control">
                    <button type="submit" v-on:click="sendData()">Add</button>
                    <div v-if="message">{{ message }}</div>
                </div>
            </div>
        </div>
    </div>
</template>
<style scoped>
.createPassword{
  gap: 1rem;
  justify-content: center;
    text-align: center;
}
</style>
<script>
export default {
    data() {
        return {
            passwordPolicies: [],
            model: {},
            validation: {},
            message: ""
        };
    },
    async mounted() {
        try {
            this.passwordPolicies = (await axios.get('passwordPolicies')).data;
        } catch (e) {
            alert('Error loading data.');
        }
    },
    methods: {
        async sendData() {
            this.validation = {};
            try {
                await axios.post('password', this.model);
                this.model = {};
                this.validation = {};
                this.message = "Your password has been saved.";
            } catch (e) {
                if (e.response.status == 400) {
                    this.validation = Object.keys(e.response.data.errors).reduce((prev, key) => {
                        const newKey = key.charAt(0).toLowerCase() + key.slice(1);
                        prev[newKey] = e.response.data.errors[key][0];
                        return prev;
                    }, {});
                    console.log(this.validation);
                }
            }
        },
    },
};
</script>
