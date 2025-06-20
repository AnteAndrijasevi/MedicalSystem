<template>
    <div class="examination-details">
        <div v-if="loading">Učitavanje...</div>
        <div v-else-if="error">{{ error }}</div>
        <div v-else>
            <h2>Pregled</h2>

            <div class="card mb-4">
                <div class="card-header">
                    <h4>Podaci o pregledu</h4>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <p><strong>Pacijent:</strong> {{ patientName }}</p>
                            <p><strong>Tip pregleda:</strong> {{ examinationTypeName }}</p>
                            <p><strong>Datum:</strong> {{ formatDate(examination.examinationDate) }}</p>
                        </div>
                        <div class="col-md-6">
                            <p><strong>Vrijeme:</strong> {{ formatTime(examination.examinationTime) }}</p>
                            <p><strong>Liječnik:</strong> {{ doctorName }}</p>
                        </div>
                    </div>

                    <div class="mt-3">
                        <h5>Bilješke</h5>
                        <p>{{ examination.notes || 'Nema bilješki' }}</p>
                    </div>
                </div>
            </div>

            <!-- Slike pregleda -->
            <div class="card mb-4">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h4>Slike</h4>
                    <button class="btn btn-primary" @click="showUploadForm = !showUploadForm">
                        {{ showUploadForm ? 'Odustani' : 'Dodaj sliku' }}
                    </button>
                </div>
                <div class="card-body">
                    <!-- Forma za upload slika -->
                    <div v-if="showUploadForm" class="mb-4">
                        <form @submit.prevent="uploadImage">
                            <div class="form-group">
                                <label for="imageFile">Odaberite sliku</label>
                                <input type="file"
                                       id="imageFile"
                                       class="form-control-file"
                                       @change="handleFileSelect"
                                       accept=".jpg,.jpeg,.png,.gif,.pdf">
                                <small class="form-text text-muted">Dozvoljeni formati: JPG, PNG, GIF, PDF</small>
                            </div>
                            <button type="submit" class="btn btn-success" :disabled="!selectedFile">Upload</button>
                        </form>
                    </div>

                    <!-- Prikaz slika -->
                    <div v-if="images.length" class="row">
                        <div v-for="image in images" :key="image.imageId" class="col-md-4 mb-3">
                            <div class="card">
                                <div class="card-body text-center">
                                    <p>{{ getFileTypeLabel(image.fileType) }}</p>
                                    <p><small>Datum: {{ formatDate(image.uploadDate) }}</small></p>
                                    <div class="btn-group">
                                        <button class="btn btn-sm btn-info" @click="viewImage(image)">Pregledaj</button>
                                        <button class="btn btn-sm btn-danger" @click="deleteImage(image)">Obriši</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div v-else class="alert alert-info">Nema slika za ovaj pregled</div>
                </div>
            </div>

            <div class="mt-3">
                <button class="btn btn-secondary" @click="goBack">Natrag</button>
                <button class="btn btn-warning ml-2" @click="editExamination">Uredi pregled</button>
            </div>
        </div>

        <!-- Modal za prikaz slike -->
        <div v-if="showImageModal" class="modal-overlay" @click="showImageModal = false">
            <div class="modal-content" @click.stop>
                <div class="modal-header">
                    <h5>Pregled slike</h5>
                    <button type="button" class="close" @click="showImageModal = false">
                        <span>&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <iframe v-if="selectedImage && isPdf(selectedImage)"
                            :src="getImageUrl(selectedImage)"
                            width="100%"
                            height="500px">
                    </iframe>
                    <img v-else-if="selectedImage"
                         :src="getImageUrl(selectedImage)"
                         alt="Medical Image"
                         class="img-fluid">
                </div>
            </div>
        </div>
    </div>
</template>

<script>import examinationService from '@/services/examinationService';
    import medicalImageService from '@/services/medicalImageService';

    export default {
        name: 'ExaminationDetails',
        props: {
            id: {
                type: [Number, String],
                required: true
            }
        },
        data() {
            return {
                examination: {},
                examinationType: {},
                patient: {},
                doctor: {},
                images: [],
                loading: true,
                error: null,
                showUploadForm: false,
                selectedFile: null,
                showImageModal: false,
                selectedImage: null
            };
        },
        computed: {
            patientName() {
                if (this.examination.patient) {
                    return `${this.examination.patient.firstName} ${this.examination.patient.lastName}`;
                }
                return 'Nepoznat';
            },

            examinationTypeName() {
                if (this.examination.examinationType) {
                    return this.examination.examinationType.name;
                } else if (this.examinationType) {
                    return this.examinationType.name;
                }
                return 'Nepoznat';
            },

            doctorName() {
                if (this.examination.doctor) {
                    return `${this.examination.doctor.firstName} ${this.examination.doctor.lastName}`;
                } else if (this.doctor) {
                    return `${this.doctor.firstName} ${this.doctor.lastName}`;
                }
                return 'Nepoznat';
            }
        },
        methods: {
            async loadExamination() {
                try {
                    const response = await examinationService.getById(this.id);
                    this.examination = response.data;
                    this.loading = false;
                } catch (error) {
                    this.error = 'Greška prilikom učitavanja podataka o pregledu';
                    this.loading = false;
                    console.error('Error loading examination:', error);
                }
            },

            async loadImages() {
                try {
                    const response = await medicalImageService.getByExamination(this.id);
                    this.images = response.data;
                } catch (error) {
                    console.error('Error loading images:', error);
                }
            },

            formatDate(dateString) {
                if (!dateString) return '';
                const date = new Date(dateString);
                return date.toLocaleDateString('hr-HR');
            },

            formatTime(timeString) {
                if (!timeString) return '';
                const parts = timeString.split(':');
                if (parts.length >= 2) {
                    return `${parts[0]}:${parts[1]}`;
                }
                return timeString;
            },

            handleFileSelect(event) {
                this.selectedFile = event.target.files[0];
            },

            async uploadImage() {
                if (!this.selectedFile) return;

                const formData = new FormData();
                formData.append('file', this.selectedFile);
                formData.append('examinationId', this.id);

                try {
                    await medicalImageService.uploadImage(formData);
                    this.selectedFile = null;
                    this.showUploadForm = false;
                    await this.loadImages();
                } catch (error) {
                    console.error('Error uploading image:', error);
                    alert('Greška prilikom uploada slike');
                }
            },

            getFileTypeLabel(fileType) {
                if (fileType.includes('image/jpeg') || fileType.includes('image/jpg')) {
                    return 'JPEG slika';
                } else if (fileType.includes('image/png')) {
                    return 'PNG slika';
                } else if (fileType.includes('image/gif')) {
                    return 'GIF slika';
                } else if (fileType.includes('application/pdf')) {
                    return 'PDF dokument';
                }
                return 'Dokument';
            },

            viewImage(image) {
                this.selectedImage = image;
                this.showImageModal = true;
            },

            getImageUrl(image) {
                return `${process.env.VUE_APP_API_URL}/medicalimages/file/${image.imageId}`;
            },

            isPdf(image) {
                return image.fileType.includes('application/pdf');
            },

            async deleteImage(image) {
                if (confirm('Jeste li sigurni da želite obrisati ovu sliku?')) {
                    try {
                        await medicalImageService.delete(image.imageId);
                        await this.loadImages();
                    } catch (error) {
                        console.error('Error deleting image:', error);
                        alert('Greška prilikom brisanja slike');
                    }
                }
            },

            goBack() {
                this.$router.go(-1);
            },

            editExamination() {
                this.$router.push(`/examinations/${this.id}/edit`);
            }
        },
        created() {
            this.loadExamination();
            this.loadImages();
        }
    };</script>

<style scoped>
    .modal-overlay {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background-color: rgba(0, 0, 0, 0.5);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 1000;
    }

    .modal-content {
        background-color: white;
        border-radius: 5px;
        width: 80%;
        max-width: 800px;
    }

    .modal-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 1rem;
        border-bottom: 1px solid #dee2e6;
    }

    .modal-body {
        padding: 1rem;
    }

    .ml-2 {
        margin-left: 0.5rem;
    }
</style>