import { axiosInstance } from "../main";

const Inversion = {
  namespaced: true,
  state: () => ({
    customers: [],
    pagos: [],
    pendingBills: [],
    typeDiscounts: [],
    mensaje: "",
    error: null,
    pago: null,
  }),
  mutations: {
    SET_CUSTOMERS: (state, datos) => {
      state.customers = datos;
    },
    SET_PAGOS: (state, datos) => {
      state.pagos = [];
      state.pagos = datos;
    },
    SET_PENDINGBILLS: (state, datos) => {
      state.pendingBills = [];
      state.pendingBills = datos;
    },
    SET_TYPESDISCOUNT: (state, datos) => {
      state.typeDiscounts = [];
      state.typeDiscounts = datos;
    },
    SET_MENSAJE(state, payload) {
      state.mensaje = payload;
    },
    SET_ERROR(state, error) {
      state.error = error;
    },
    DELETEITEM(state, item) {
      const index = state.pendingBills.indexOf(item);
      state.pendingBills.splice(index, 1);
    },
    ADDITEM(state, item) {
      state.pendingBills.push(item);
    },
    SET_PAGO(state, pago) {
      state.pago = pago;
    },
    REMOVE_PAGO(state, folioPago) {
      state.pagos = state.pagos.filter((p) => p.folioPago !== folioPago);
    },
  },
  actions: {
    limpiarCredito: ({ commit }) => {
      commit("SET_CUSTOMERS", []);
      commit("SET_PAGOS", []);
      commit("SET_PENDINGBILLS", []);
    },
    deleteItemPending: ({ commit }, item) => {
      commit("DELETEITEM", item);
    },
    addItemPending: ({ commit }, item) => {
      commit("ADDITEM", item);
    },
    getSaldoFijo: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(`/api/Investment/saldofijo`)
          .then((res) => {
            resolve(res.data);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },

    async postSaldoFijo({ commit }, saldoFijo) {
      try {
        const response = await axiosInstance.post(
          "/api/Investment/saldofijo",
          saldoFijo
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },

    async putSaldoFijo({ commit }, saldoFijo) {
      try {
        const id = saldoFijo.id !== undefined ? saldoFijo.id : saldoFijo.Id;
        const response = await axiosInstance.put(
          `/api/Investment/saldofijo/${id}`,
          saldoFijo
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },

    async deleteSaldoFijo({ commit }, id) {
      try {
        const response = await axiosInstance.delete(
          `/api/Investment/saldofijo/${id}`
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },

    async postSaldoDisponible({ commit }, pago) {
      try {
        const response = await axiosInstance.post(
          "/api/Investment/saldodisponible",
          pago
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async getSaldoDisponible({ commit }, fecha) {
      try {
        const response = await axiosInstance.get(
          "/api/Investment/saldodisponible?fecha=" + fecha
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async deleteSaldoDisponible({ commit }, id) {
      try {
        const response = await axiosInstance.delete(
          "/api/Investment/saldodisponible/" + id
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },

    // OBTENER APARTADOS
    async getApartados({ commit }, fecha) {
      try {
        const response = await axiosInstance.get(
          `/api/Investment/apartados?fecha=${fecha}`
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },

    // GUARDAR UNO MANUAL
    async postApartado({ commit }, payload) {
      try {
        const response = await axiosInstance.post(
          "/api/Investment/apartados/manual",
          payload
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },

    // GUARDAR LISTA DESDE EXCEL
    async postApartadosExcel({ commit }, payload) {
      try {
        const response = await axiosInstance.post(
          "/api/Investment/apartados/bulk",
          payload
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    // ACTUALIZAR (PUT)
    async updateApartado({ commit }, apartado) {
      try {
        const id = apartado.id !== undefined ? apartado.id : apartado.Id;
        const response = await axiosInstance.put(
          `/api/Investment/apartados/${id}`,
          apartado
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },

    // ELIMINAR (DELETE)
    async deleteApartado({ commit }, id) {
      try {
        const response = await axiosInstance.delete(
          `/api/Investment/apartados/${id}`
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    // OBTENER CATALOGO DE CUENTAS
    async getCuentas({ commit }) {
      try {
        const response = await axiosInstance.get(
          `/api/Investment/cuentas/orden`
        );
        commit("SET_ERROR", null);
        return response.data; // Retorna array de objects { almacen, cuenta }
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
  },
};

export default Inversion;
