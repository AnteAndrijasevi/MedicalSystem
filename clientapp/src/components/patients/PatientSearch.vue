<template>
    <div class="patient-search mb-4">
        <div class="input-group">
            <input type="text"
                   class="form-control"
                   v-model="searchTerm"
                   placeholder="Pretraži po prezimenu ili OIB-u..."
                   @keyup.enter="search">
            <div class="input-group-append">
                <button class="btn btn-outline-secondary" type="button" @click="search">Pretraži</button>
                <button class="btn btn-outline-secondary" type="button" @click="clear">Očisti</button>
            </div>
        </div>
    </div>
</template>

<script>export default {
        name: 'PatientSearch',
        data() {
            return {
                searchTerm: ''
            };
        },
        methods: {
            search() {
                this.$emit('search', this.searchTerm);
            },
            clear() {
                this.searchTerm = '';
                this.$emit('search', '');
            }
        }
    };</script>