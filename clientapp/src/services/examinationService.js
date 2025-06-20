import api from './api';

export default {
    getAll() {
        return api.get('/examinations');
    },

    getById(id) {
        return api.get(`/examinations/${id}`);
    },

    getByPatient(patientId) {
        return api.get(`/examinations/patient/${patientId}`);
    },

    getTypes() {
        return api.get('/examinations/types');
    },

    create(examination) {
        return api.post('/examinations', examination);
    },

    update(id, examination) {
        return api.put(`/examinations/${id}`, examination);
    },

    delete(id) {
        return api.delete(`/examinations/${id}`);
    }
};