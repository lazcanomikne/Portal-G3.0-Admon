import { axiosInstance } from "../main";

const ConfigCuentas = {
    namespaced: true,
    state: () => ({
        principales: [],
        selectedOrdenId: null, // ID seleccionado actualmente
        dependencias: { usd: [], ref: [] },
        loading: false
    }),
    mutations: {
        SET_PRINCIPALES(state, data) { state.principales = data; },
        SET_SELECTED(state, id) { state.selectedOrdenId = id; },
        SET_DEPENDENCIAS(state, data) {
            state.dependencias.usd = data.cuentasUSD;
            state.dependencias.ref = data.cuentasRef;
        },
        SET_LOADING(state, val) { state.loading = val; }
    },
    actions: {
        async getPrincipales({ commit }) {
            commit('SET_LOADING', true);
            try {
                const res = await axiosInstance.get('/api/ConfigCuentas/principales');
                commit('SET_PRINCIPALES', res.data);
            } catch (error) {
                console.error("Error fetching principales:", error);
            } finally {
                commit('SET_LOADING', false);
            }
        },
        async addPrincipal({ dispatch }, payload) {
            try {
                await axiosInstance.post('/api/ConfigCuentas/principales', payload);
                dispatch('getPrincipales');
            } catch (error) {
                console.error("Error adding principal:", error);
                throw error;
            }
        },
        async loadDependencias({ commit }, ordenId) {
            commit('SET_SELECTED', ordenId); // Marcamos cual estamos viendo
            try {
                const res = await axiosInstance.get(`/api/ConfigCuentas/dependencias/${ordenId}`);
                commit('SET_DEPENDENCIAS', res.data);
            } catch (error) {
                console.error("Error loading dependencias:", error);
            }
        },
        async addDependencia({ dispatch, state }, { type, payload }) {
            // type: 'usd' o 'referenciada'
            // Inyectamos el ID padre automaticamente
            payload.OrdenID = state.selectedOrdenId;
            try {
                await axiosInstance.post(`/api/ConfigCuentas/${type}`, payload);
                dispatch('loadDependencias', state.selectedOrdenId);
            } catch (error) {
                console.error("Error adding dependencia:", error);
                throw error;
            }
        }
    }
};
export default ConfigCuentas;
