import api from './api';

export default {
    getAll() {
        return api.get('/medicalimages');
    },

    getById(id) {
        return api.get(`/medicalimages/${id}`);
    },

    getByExamination(examinationId) {
        return api.get(`/medicalimages/examination/${examinationId}`);
    },

    uploadImage(formData) {
        return api.post('/medicalimages', formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    },

    delete(id) {
        return api.delete(`/medicalimages/${id}`);
    }
};