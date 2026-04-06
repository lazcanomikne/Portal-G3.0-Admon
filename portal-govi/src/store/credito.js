import { axiosInstance } from "../main";

const Credito = {
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
    getCustomers: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(
            `/api/credit/customers?sociedad=${data.sociedad}&sucursal=${data.sucursal}`
          )
          .then((res) => {
            commit("SET_CUSTOMERS", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    getPagosCta: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(
            `/api/credit/pagocta?sociedad=${data.sociedad}&cuenta=${data.cuenta}`
          )
          .then((res) => {
            commit("SET_PAGOS", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    getPendingBill: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(
            `/api/credit/pendingbill?sociedad=${data.sociedad}&sucursal=${data.sucursal}&cliente=${data.cliente}`
          )
          .then((res) => {
            commit("SET_PENDINGBILLS", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    getSaldoFavor: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(`/api/credit/saldoFavor?cliente=${data.cliente}`)
          .then((res) => {
            resolve(res.data);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    getTypeDiscounts: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(`/api/credit/typediscount?sociedad=${data.sociedad}`)
          .then((res) => {
            commit("SET_TYPESDISCOUNT", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    async insertarPago({ commit }, pago) {
      try {
        const response = await axiosInstance.post("/api/credit/pagos", pago);
        commit(
          "SET_MENSAJE",
          `Pago insertado con Folio: ${response.data.folio}`
        );
        commit("SET_ERROR", null);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async getReportHeader({ commit }, fecha) {
      try {
        const response = await axiosInstance.get(
          `/api/credit/report/header?fecha=${fecha}`
        );
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async getReportDetail({ commit }, folio) {
      try {
        const response = await axiosInstance.get(
          `/api/credit/report/details?folio=${folio}`
        );
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async getReportHeaderCuadroInversion({ commit }, fecha) {
      try {
        const response = await axiosInstance.get(
          `/api/credit/report/header/cuadroinversion?fecha=${fecha}`
        );
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async getReportDetailCuadroInversion({ commit }, cuentas) {
      try {
        const response = await axiosInstance.get(
          `/api/credit/report/details/cuadroinversion?cuentas=${cuentas}`
        );
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async getAutorizacionPreaplicaciones({ commit }) {
      try {
        const response = await axiosInstance.get(
          `/api/credit/operation/header`
        );
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async updateAutorizacionPreaplicaciones({ commit }, folio) {
      try {
        const response = await axiosInstance.put(
          `/api/credit/operation/header?folio=${folio}`
        );
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async getPagoByFolio({ commit }, folioPago) {
      try {
        const response = await axiosInstance.get(
          `/api/credit/pagos/${folioPago}`
        );
        commit("SET_PAGO", response.data);
        return response.data;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
    async deletePagoByFolio({ commit }, folioPago) {
      try {
        await axiosInstance.delete(`/api/credit/pagos/${folioPago}`);
        commit("REMOVE_PAGO", folioPago);
        return true;
      } catch (error) {
        commit("SET_ERROR", error.response?.data || error.message);
        throw error;
      }
    },
  },
};

export default Credito;
