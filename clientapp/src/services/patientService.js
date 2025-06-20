import api from './api';

export default {
    getAll() {
        return api.get('/patients');
    },

    getById(id) {
        return api.get(`/patients/${id}`);
    },

    create(patient) {
        return api.post('/patients', patient);
    },

    update(id, patient) {
        return api.put(`/patients/${id}`, patient);
    },

    delete(id) {
        return api.delete(`/patients/${id}`);
    },

    search(term) {
        return api.get(`/patients/search?term=${term}`);
    },

    exportToCsv(id) {
        return api.get(`/patients/${id}/export`, { responseType: 'blob' });
    }
};