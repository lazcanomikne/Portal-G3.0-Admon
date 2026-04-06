import { axiosInstance } from "../main";

const Usuarios = {
    namespaced: true,
    state: () => ({
        usuarios: [],
        menuTree: [], // Para guardar la estructura de permisos al editar
        loading: false
    }),
    mutations: {
        SET_USUARIOS(state, data) { state.usuarios = data; },
        SET_MENU_TREE(state, data) { state.menuTree = data; },
        SET_LOADING(state, val) { state.loading = val; }
    },
    actions: {
        async getUsuarios({ commit }) {
            commit('SET_LOADING', true);
            try {
                const res = await axiosInstance.get('/api/Users');
                commit('SET_USUARIOS', res.data);
            } finally {
                commit('SET_LOADING', false);
            }
        },
        async createUsuario({ dispatch }, payload) {
            await axiosInstance.post('/api/Users/create', payload);
            dispatch('getUsuarios');
        },
        async testSapLogin(_, payload) {
            // Retorna la promesa para manejar success/fail en la vista
            return axiosInstance.post('/api/Users/test-sap', payload);
        },
        async getPermissions({ commit }, username) {
            const res = await axiosInstance.get(`/api/Users/permissions/${username}`);
            commit('SET_MENU_TREE', res.data);
            return res.data;
        },
        async savePermissions(_, { username, permissions }) {
            await axiosInstance.post(`/api/Users/permissions/${username}`, permissions);
        }
    }
};

export default Usuarios;
