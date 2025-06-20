<template>
    <div class="patient-details">
        <div v-if="loading">Učitavanje...</div>
        <div v-else-if="error">{{ error }}</div>
        <div v-else>
            <h2>{{ patient.firstName }} {{ patient.lastName }}</h2>

            <div class="card mb-4">
                <div class="card-header">
                    <h4>Osnovni podaci</h4>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <p><strong>OIB:</strong> {{ patient.oib }}</p>
                            <p><strong>Broj pacijenta:</strong> {{ patient.patientNumber }}</p>
                            <p><strong>Datum rođenja:</strong> {{ formatDate(patient.dateOfBirth) }}</p>
                        </div>
                        <div class="col-md-6">
                            <p><strong>Spol:</strong> {{ formatGender(patient.gender) }}</p>
                            <p><strong>Kontakt:</strong> {{ patient.contactNumber }}</p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Tabs za različite kategorije podataka -->
            <ul class="nav nav-tabs mb-3">
                <li class="nav-item">
                    <a class="nav-link" :class="{ active: activeTab === 'history' }" href="#" @click.prevent="activeTab = 'history'">Povijest bolesti</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" :class="{ active: activeTab === 'examinations' }" href="#" @click.prevent="activeTab = 'examinations'">Pregledi</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" :class="{ active: activeTab === 'prescriptions' }" href="#" @click.prevent="activeTab = 'prescriptions'">Recepti</a>
                </li>
            </ul>

            <!-- Prikaz podataka prema odabranom tabu -->
            <div class="tab-content">
                <!-- Povijest bolesti -->
                <div v-if="activeTab === 'history'" class="tab-pane active">
                    <div class="d-flex justify-content-between mb-3">
                        <h4>Povijest bolesti</h4>
                        <button class="btn btn-sm btn-primary" @click="addMedicalHistory">Dodaj povijest bolesti</button>
                    </div>

                    <table class="table table-striped" v-if="medicalHistories.length">
                        <thead>
                            <tr>
                                <th>Bolest</th>
                                <th>Datum početka</th>
                                <th>Datum završetka</th>
                                <th>Akcije</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="history in medicalHistories" :key="history.medicalHistoryId">
                                <td>{{ history.diseaseName }}</td>
                                <td>{{ formatDate(history.startDate) }}</td>
                                <td>{{ history.endDate ? formatDate(history.endDate) : 'Aktivno' }}</td>
                                <td>
                                    <button class="btn btn-sm btn-warning" @click="editMedicalHistory(history)">Uredi</button>
                                    <button class="btn btn-sm btn-danger" @click="deleteMedicalHistory(history)">Obriši</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div v-else class="alert alert-info">Nema zapisa o povijesti bolesti</div>
                </div>

                <!-- Pregledi -->
                <div v-if="activeTab === 'examinations'" class="tab-pane active">
                    <div class="d-flex justify-content-between mb-3">
                        <h4>Pregledi</h4>
                        <button class="btn btn-sm btn-primary" @click="addExamination">Dodaj pregled</button>
                    </div>

                    <table class="table table-striped" v-if="examinations.length">
                        <thead>
                            <tr>
                                <th>Tip pregleda</th>
                                <th>Datum</th>
                                <th>Vrijeme</th>
                                <th>Bilješke</th>
                                <th>Akcije</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="exam in examinations" :key="exam.examinationId">
                                <td>{{ getExaminationTypeName(exam.typeId) }}</td>
                                <td>{{ formatDate(exam.examinationDate) }}</td>
                                <td>{{ formatTime(exam.examinationTime) }}</td>
                                <td>{{ exam.notes }}</td>
                                <td>
                                    <div class="btn-group">
                                        <button class="btn btn-sm btn-info" @click="viewExamination(exam)">Detalji</button>
                                        <button class="btn btn-sm btn-warning" @click="editExamination(exam)">Uredi</button>
                                        <button class="btn btn-sm btn-danger" @click="deleteExamination(exam)">Obriši</button>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div v-else class="alert alert-info">Nema zapisa o pregledima</div>
                </div>

                <!-- Recepti -->
                <div v-if="activeTab === 'prescriptions'" class="tab-pane active">
                    <div class="d-flex justify-content-between mb-3">
                        <h4>Recepti</h4>
                        <button class="btn btn-sm btn-primary" @click="addPrescription">Dodaj recept</button>
                    </div>

                    <table class="table table-striped" v-if="prescriptions.length">
                        <thead>
                            <tr>
                                <th>Lijek</th>
                                <th>Doziranje</th>
                                <th>Datum izdavanja</th>
                                <th>Upute</th>
                                <th>Akcije</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="prescription in prescriptions" :key="prescription.prescriptionId">
                                <td>{{ prescription.medicationName }}</td>
                                <td>{{ prescription.dosage }}</td>
                                <td>{{ formatDate(prescription.issueDate) }}</td>
                                <td>{{ prescription.instructions }}</td>
                                <td>
                                    <button class="btn btn-sm btn-warning" @click="editPrescription(prescription)">Uredi</button>
                                    <button class="btn btn-sm btn-danger" @click="deletePrescription(prescription)">Obriši</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div v-else class="alert alert-info">Nema zapisa o receptima</div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>import patientService from '@/services/patientService';
    import medicalHistoryService from '@/services/medicalHistoryService';
    import examinationService from '@/services/examinationService';
    import prescriptionService from '@/services/prescriptionService';

    export default {
        name: 'PatientDetails',
        props: {
            id: {
                type: [Number, String],
                required: true
            }
        },
        data() {
            return {
                patient: {},
                medicalHistories: [],
                examinations: [],
                prescriptions: [],
                examinationTypes: [],
                activeTab: 'history',
                loading: true,
                error: null
            };
        },
        methods: {
            async loadPatient() {
                try {
                    const response = await patientService.getById(this.id);
                    this.patient = response.data;
                    this.loading = false;
                } catch (error) {
                    this.error = 'Greška prilikom učitavanja podataka o pacijentu';
                    this.loading = false;
                    console.error('Error loading patient:', error);
                }
            },

            async loadMedicalHistories() {
                try {
                    const response = await medicalHistoryService.getByPatient(this.id);
                    this.medicalHistories = response.data;
                } catch (error) {
                    console.error('Error loading medical histories:', error);
                }
            },

            async loadExaminations() {
                try {
                    const response = await examinationService.getByPatient(this.id);
                    this.examinations = response.data;

                    // Load examination types
                    const typesResponse = await examinationService.getTypes();
                    this.examinationTypes = typesResponse.data;
                } catch (error) {
                    console.error('Error loading examinations:', error);
                }
            },

            async loadPrescriptions() {
                try {
                    const response = await prescriptionService.getByPatient(this.id);
                    this.prescriptions = response.data;
                } catch (error) {
                    console.error('Error loading prescriptions:', error);
                }
            },

            getExaminationTypeName(typeId) {
                const type = this.examinationTypes.find(t => t.typeId === typeId);
                return type ? type.name : `Tip ${typeId}`;
            },

            formatDate(dateString) {
                if (!dateString) return '';
                const date = new Date(dateString);
                return date.toLocaleDateString('hr-HR');
            },

            formatTime(timeString) {
                if (!timeString) return '';
                // Parse time in format HH:MM:SS
                const parts = timeString.split(':');
                if (parts.length >= 2) {
                    return `${parts[0]}:${parts[1]}`;
                }
                return timeString;
            },

            formatGender(gender) {
                switch (gender) {
                    case 'M': return 'Muško';
                    case 'F': return 'Žensko';
                    case 'O': return 'Ostalo';
                    default: return gender;
                }
            },

            // Akcije za povijest bolesti
            addMedicalHistory() {
                this.$router.push(`/patients/${this.id}/medical-history/new`);
            },

            editMedicalHistory(history) {
                this.$router.push(`/patients/${this.id}/medical-history/${history.medicalHistoryId}/edit`);
            },

            async deleteMedicalHistory(history) {
                if (confirm('Jeste li sigurni da želite obrisati ovaj zapis povijesti bolesti?')) {
                    try {
                        await medicalHistoryService.delete(history.medicalHistoryId);
                        await this.loadMedicalHistories();
                    } catch (error) {
                        console.error('Error deleting medical history:', error);
                        alert('Greška prilikom brisanja povijesti bolesti');
                    }
                }
            },

            // Akcije za preglede
            addExamination() {
                this.$router.push(`/patients/${this.id}/examinations/new`);
            },

            viewExamination(exam) {
                this.$router.push(`/examinations/${exam.examinationId}`);
            },

            editExamination(exam) {
                this.$router.push(`/examinations/${exam.examinationId}/edit`);
            },

            async deleteExamination(exam) {
                if (confirm('Jeste li sigurni da želite obrisati ovaj pregled?')) {
                    try {
                        await examinationService.delete(exam.examinationId);
                        await this.loadExaminations();
                    } catch (error) {
                        console.error('Error deleting examination:', error);
                        alert('Greška prilikom brisanja pregleda');
                    }
                }
            },

            // Akcije za recepte
            addPrescription() {
                this.$router.push(`/patients/${this.id}/prescriptions/new`);
            },

            editPrescription(prescription) {
                this.$router.push(`/prescriptions/${prescription.prescriptionId}/edit`);
            },

            async deletePrescription(prescription) {
                if (confirm('Jeste li sigurni da želite obrisati ovaj recept?')) {
                    try {
                        await prescriptionService.delete(prescription.prescriptionId);
                        await this.loadPrescriptions();
                    } catch (error) {
                        console.error('Error deleting prescription:', error);
                        alert('Greška prilikom brisanja recepta');
                    }
                }
            }
        },
        watch: {
            activeTab(newTab) {
                if (newTab === 'history') {
                    this.loadMedicalHistories();
                } else if (newTab === 'examinations') {
                    this.loadExaminations();
                } else if (newTab === 'prescriptions') {
                    this.loadPrescriptions();
                }
            }
        },
        created() {
            this.loadPatient();
            this.loadMedicalHistories();
        }
    };</script>