<template>
    <div id="app">
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">

        <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
            <div class="container">
                <a class="navbar-brand" href="#">Medical System</a>
            </div>
        </nav>

        <div class="container mt-4">
            <div class="row">
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header">
                            <h3>API Connection Test</h3>
                        </div>
                        <div class="card-body">
                            <button @click="testAPI" class="btn btn-primary" :disabled="loading">
                                {{ loading ? 'Testing...' : 'Test API Connection' }}
                            </button>
                            <div v-if="apiStatus" class="mt-3">
                                <div class="alert" :class="apiStatus.success ? 'alert-success' : 'alert-danger'">
                                    {{ apiStatus.message }}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="card">
                        <div class="card-header">
                            <h3>Database Stats</h3>
                        </div>
                        <div class="card-body">
                            <button @click="loadStats" class="btn btn-info" :disabled="loading">
                                {{ loading ? 'Loading...' : 'Load Statistics' }}
                            </button>
                            <div v-if="stats" class="mt-3">
                                <ul class="list-group">
                                    <li class="list-group-item d-flex justify-content-between align-items-center">
                                        Patients
                                        <span class="badge bg-primary rounded-pill">{{ stats.patients || 0 }}</span>
                                    </li>
                                    <li class="list-group-item d-flex justify-content-between align-items-center">
                                        Examinations
                                        <span class="badge bg-secondary rounded-pill">{{ stats.examinations || 0 }}</span>
                                    </li>
                                    <li class="list-group-item d-flex justify-content-between align-items-center">
                                        Medical Histories
                                        <span class="badge bg-success rounded-pill">{{ stats.histories || 0 }}</span>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Patients Section -->
            <div class="row mt-4">
                <div class="col-12">
                    <div class="card">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h3>Patients Management</h3>
                            <button @click="showAddPatient = !showAddPatient" class="btn btn-success">
                                {{ showAddPatient ? 'Cancel' : 'Add Patient' }}
                            </button>
                        </div>
                        <div class="card-body">
                            <!-- Add Patient Form -->
                            <div v-if="showAddPatient" class="mb-4">
                                <h5>Add New Patient</h5>
                                <form @submit.prevent="addPatient">
                                    <div class="row">
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">First Name</label>
                                            <input type="text" class="form-control" v-model="newPatient.firstName" required>
                                        </div>
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Last Name</label>
                                            <input type="text" class="form-control" v-model="newPatient.lastName" required>
                                        </div>
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">OIB (11 digits)</label>
                                            <input type="text" class="form-control" v-model="newPatient.oib"
                                                   pattern="[0-9]{11}" maxlength="11" required>
                                        </div>
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Date of Birth</label>
                                            <input type="date" class="form-control" v-model="newPatient.dateOfBirth" required>
                                        </div>
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Gender</label>
                                            <select class="form-control" v-model="newPatient.gender" required>
                                                <option value="">Select Gender</option>
                                                <option value="M">Male</option>
                                                <option value="F">Female</option>
                                                <option value="O">Other</option>
                                            </select>
                                        </div>
                                        <div class="col-md-6 mb-3">
                                            <label class="form-label">Contact Number</label>
                                            <input type="text" class="form-control" v-model="newPatient.contactNumber">
                                        </div>
                                    </div>
                                    <button type="submit" class="btn btn-primary">Add Patient</button>
                                </form>
                            </div>

                            <!-- Search -->
                            <div class="mb-3">
                                <div class="input-group">
                                    <input type="text" class="form-control" v-model="searchTerm"
                                           placeholder="Search by name or OIB..." @keyup.enter="searchPatients">
                                    <button class="btn btn-outline-secondary" @click="searchPatients">Search</button>
                                    <button class="btn btn-outline-secondary" @click="clearSearch">Clear</button>
                                </div>
                            </div>

                            <!-- Load Patients Button -->
                            <button @click="loadPatients" class="btn btn-primary mb-3" :disabled="loading">
                                {{ loading ? 'Loading...' : 'Load All Patients' }}
                            </button>

                            <!-- Patients Table -->
                            <div v-if="patients.length > 0" class="table-responsive">
                                <table class="table table-striped">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Name</th>
                                            <th>OIB</th>
                                            <th>Date of Birth</th>
                                            <th>Gender</th>
                                            <th>Contact</th>
                                            <th>Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="patient in patients" :key="patient.patientId">
                                            <td>{{ patient.patientId }}</td>
                                            <td>{{ patient.firstName }} {{ patient.lastName }}</td>
                                            <td>{{ patient.oib }}</td>
                                            <td>{{ formatDate(patient.dateOfBirth) }}</td>
                                            <td>{{ formatGender(patient.gender) }}</td>
                                            <td>{{ patient.contactNumber || 'N/A' }}</td>
                                            <td>
                                                <button class="btn btn-sm btn-danger" @click="deletePatient(patient.patientId)">
                                                    Delete
                                                </button>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>

                            <div v-else-if="!loading" class="alert alert-info">
                                No patients found. Click "Load All Patients" to see patients or add a new one.
                            </div>

                            <div v-if="error" class="alert alert-danger">
                                {{ error }}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>import axios from 'axios';

    const api = axios.create({
        baseURL: 'https://localhost:7282/api',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    export default {
        name: 'App',
        data() {
            return {
                apiStatus: null,
                stats: null,
                patients: [],
                searchTerm: '',
                loading: false,
                error: null,
                showAddPatient: false,
                newPatient: {
                    firstName: '',
                    lastName: '',
                    oib: '',
                    dateOfBirth: '',
                    gender: '',
                    contactNumber: ''
                }
            };
        },
        methods: {
            async testAPI() {
                this.loading = true;
                try {
                    const response = await fetch('https://localhost:7282/api/test');
                    if (response.ok) {
                        const text = await response.text();
                        this.apiStatus = {
                            success: true,
                            message: `✅ API Connection successful: ${text}`
                        };
                    } else {
                        this.apiStatus = {
                            success: false,
                            message: `❌ API returned status: ${response.status}`
                        };
                    }
                } catch (error) {
                    this.apiStatus = {
                        success: false,
                        message: `❌ Connection failed: ${error.message}`
                    };
                } finally {
                    this.loading = false;
                }
            },

            async loadStats() {
                this.loading = true;
                try {
                    const [patientsRes, examinationsRes, historiesRes] = await Promise.all([
                        api.get('/patients'),
                        api.get('/examinations'),
                        api.get('/medicalhistories')
                    ]);

                    this.stats = {
                        patients: patientsRes.data.length,
                        examinations: examinationsRes.data.length,
                        histories: historiesRes.data.length
                    };
                } catch (error) {
                    console.error('Error loading stats:', error);
                    this.error = 'Failed to load statistics';
                } finally {
                    this.loading = false;
                }
            },

            async loadPatients() {
                this.loading = true;
                this.error = null;
                try {
                    const response = await api.get('/patients');
                    this.patients = response.data;
                } catch (error) {
                    console.error('Error loading patients:', error);
                    this.error = `Failed to load patients: ${error.message}`;
                } finally {
                    this.loading = false;
                }
            },

            async searchPatients() {
                if (!this.searchTerm.trim()) {
                    this.loadPatients();
                    return;
                }

                this.loading = true;
                this.error = null;
                try {
                    const response = await api.get(`/patients/search?term=${this.searchTerm}`);
                    this.patients = response.data;
                } catch (error) {
                    console.error('Error searching patients:', error);
                    this.error = `Search failed: ${error.message}`;
                } finally {
                    this.loading = false;
                }
            },

            clearSearch() {
                this.searchTerm = '';
                this.loadPatients();
            },

            async addPatient() {
                this.loading = true;
                this.error = null;
                try {
                    await api.post('/patients', this.newPatient);

                    // Reset form
                    this.newPatient = {
                        firstName: '',
                        lastName: '',
                        oib: '',
                        dateOfBirth: '',
                        gender: '',
                        contactNumber: ''
                    };

                    this.showAddPatient = false;
                    this.loadPatients(); // Reload patients list

                    alert('Patient added successfully!');
                } catch (error) {
                    console.error('Error adding patient:', error);
                    this.error = `Failed to add patient: ${error.response?.data?.message || error.message}`;
                } finally {
                    this.loading = false;
                }
            },

            async deletePatient(patientId) {
                if (!confirm('Are you sure you want to delete this patient?')) {
                    return;
                }

                try {
                    await api.delete(`/patients/${patientId}`);
                    this.loadPatients(); // Reload patients list
                    alert('Patient deleted successfully!');
                } catch (error) {
                    console.error('Error deleting patient:', error);
                    this.error = `Failed to delete patient: ${error.message}`;
                }
            },

            formatDate(dateString) {
                if (!dateString) return '';
                const date = new Date(dateString);
                return date.toLocaleDateString('en-GB');
            },

            formatGender(gender) {
                switch (gender) {
                    case 'M': return 'Male';
                    case 'F': return 'Female';
                    case 'O': return 'Other';
                    default: return gender;
                }
            }
        },

        mounted() {
            this.testAPI();
        }
    };</script>

<style>
    #app {
        font-family: Avenir, Helvetica, Arial, sans-serif;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
    }

    .container {
        max-width: 1200px;
    }
</style>