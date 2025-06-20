<template>
    <div class="patient-list">
        <div class="d-flex justify-content-between mb-3">
            <h2>Pacijenti</h2>
            <button class="btn btn-primary" @click="$router.push('/patients/new')">Novi pacijent</button>
        </div>

        <patient-search @search="searchPatients" />

        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Broj pacijenta</th>
                    <th>Ime</th>
                    <th>Prezime</th>
                    <th>OIB</th>
                    <th>Datum rođenja</th>
                    <th>Spol</th>
                    <th>Kontakt</th>
                    <th>Akcije</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="patient in patients" :key="patient.patientId">
                    <td>{{ patient.patientNumber }}</td>
                    <td>{{ patient.firstName }}</td>
                    <td>{{ patient.lastName }}</td>
                    <td>{{ patient.oib }}</td>
                    <td>{{ formatDate(patient.dateOfBirth) }}</td>
                    <td>{{ formatGender(patient.gender) }}</td>
                    <td>{{ patient.contactNumber }}</td>
                    <td>
                        <div class="btn-group">
                            <button class="btn btn-sm btn-info" @click="viewDetails(patient)">Detalji</button>
                            <button class="btn btn-sm btn-warning" @click="editPatient(patient)">Uredi</button>
                            <button class="btn btn-sm btn-danger" @click="confirmDelete(patient)">Obriši</button>
                            <button class="btn btn-sm btn-secondary" @click="exportPatient(patient)">Izvoz CSV</button>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script>import PatientSearch from './PatientSearch.vue';
    import patientService from '@/services/patientService';

    export default {
        name: 'PatientList',
        components: {
            PatientSearch
        },
        data() {
            return {
                patients: []
            };
        },
        methods: {
            async loadPatients() {
                try {
                    const response = await patientService.getAll();
                    this.patients = response.data;
                } catch (error) {
                    console.error('Error loading patients:', error);
                    alert('Greška prilikom učitavanja pacijenata');
                }
            },

            async searchPatients(term) {
                try {
                    if (!term) {
                        this.loadPatients();
                        return;
                    }

                    const response = await patientService.search(term);
                    this.patients = response.data;
                } catch (error) {
                    console.error('Error searching patients:', error);
                    alert('Greška prilikom pretraživanja pacijenata');
                }
            },

            viewDetails(patient) {
                this.$router.push(`/patients/${patient.patientId}`);
            },

            editPatient(patient) {
                this.$router.push(`/patients/${patient.patientId}/edit`);
            },

            async confirmDelete(patient) {
                if (confirm(`Jeste li sigurni da želite obrisati pacijenta ${patient.firstName} ${patient.lastName}?`)) {
                    try {
                        await patientService.delete(patient.patientId);
                        this.loadPatients();
                    } catch (error) {
                        console.error('Error deleting patient:', error);
                        alert('Greška prilikom brisanja pacijenta');
                    }
                }
            },

            async exportPatient(patient) {
                try {
                    const response = await patientService.exportToCsv(patient.patientId);
                    const url = window.URL.createObjectURL(new Blob([response.data]));
                    const link = document.createElement('a');
                    link.href = url;
                    link.setAttribute('download', `patient_${patient.patientId}_data.csv`);
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                } catch (error) {
                    console.error('Error exporting patient data:', error);
                    alert('Greška prilikom izvoza podataka pacijenta');
                }
            },

            formatDate(dateString) {
                if (!dateString) return '';
                const date = new Date(dateString);
                return date.toLocaleDateString('hr-HR');
            },

            formatGender(gender) {
                switch (gender) {
                    case 'M': return 'Muško';
                    case 'F': return 'Žensko';
                    case 'O': return 'Ostalo';
                    default: return gender;
                }
            }
        },
        created() {
            this.loadPatients();
        }
    };
    </script>