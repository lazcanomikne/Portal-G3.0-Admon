import { axiosInstance } from "../main";

const Informes = {
  namespaced: true,
  state: () => ({
    sdaldia: [],
    foliosGenerados: [],
    infoTransfers: [],
    detallTransfers: [],
    rows: [],
    cols: [],
    uuidlist: [],
    parameters: [],
    loadingInfo: false,
    title: "",
  }),
  mutations: {
    SET_SD: (state, datos) => {
      state.sdaldia = datos;
    },
    SET_INFOTRANSFERS: (state, datos) => {
      state.infoTransfers = datos;
    },
    SET_DETAILSTRANSFERS: (state, datos) => {
      state.detallTransfers = datos;
    },
    SET_ROWS: (state, datos) => {
      state.rows = datos;
    },
    SET_COLS: (state, datos) => {
      state.cols = datos;
    },
    SET_PROPS: (state, datos) => {
      state.parameters = datos;
    },
    SET_CARGANDO: (state, datos) => {
      state.loadingInfo = datos;
    },
    SET_TITLE: (state, datos) => {
      state.title = datos;
    },
    SET_UUIDSTATE: (state, datos) => {
      state.uuidlist = datos;
    },
  },
  actions: {
    limpiar: ({ commit }) => {
      commit("SET_COLS", []);
      commit("SET_PROPS", []);
      commit("SET_ROWS", []);
      commit("SET_UUIDSTATE", []);
    },
    getDatos: ({ commit }, fecha) => {
      commit("SET_SD", []);
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(`/api/dataapp/sdaldia?fecha=${fecha}`)
          .then((res) => {
            commit("SET_SD", res.data);
            resolve();
          })
          .catch(() => reject());
      });
    },
    getInfoTransfers: ({ commit }, fecha) => {
      commit("SET_INFOTRANSFERS", []);
      return new Promise((resolve, reject) => {
        axiosInstance
          .post(`/api/dataapp/infotransfers`, fecha)
          .then((res) => {
            if (res.data.length > 0) {
              commit("SET_INFOTRANSFERS", res.data);
              resolve();
            } else reject();
          })
          .catch(() => reject());
      });
    },
    getDetailsTransfers: ({ commit }, data) => {
      commit("SET_DETAILSTRANSFERS", []);
      return new Promise((resolve, reject) => {
        axiosInstance
          .post(
            `/api/dataapp/detailstransfers?empresa=${data.empresa}`,
            data.fechas
          )
          .then((res) => {
            commit("SET_DETAILSTRANSFERS", res.data);
            resolve();
          })
          .catch(() => reject());
      });
    },
    getInforme: async ({ commit }, params) => {
      commit("SET_ROWS", []);
      try {
        commit("SET_CARGANDO", true);
        const req = await axiosInstance.get(
          `/api/dataapp/informe?informe=${params.id}`
        );
        const data = await req.data;
        commit("SET_ROWS", data.rows);
        commit("SET_CARGANDO", false);
      } catch (error) {
        commit("SET_CARGANDO", false);
        console.log(error);
      }
    },
    getParametros: async ({ commit }, id) => {
      commit("SET_COLS", []);
      commit("SET_PROPS", []);
      try {
        const req = await axiosInstance.get(
          `/api/dataapp/parametros?informe=${id}`
        );
        const data = await req.data;
        commit("SET_COLS", data.cols);
        commit("SET_PROPS", data.props);
      } catch (error) {
        console.log(error);
      }
    },
    getUUIDStatus: async ({ commit }, date) => {
      commit("SET_UUIDSTATE", []);
      try {
        commit("SET_CARGANDO", true);
        const req = await axiosInstance.get(`/api/Cancelacion?fecha=${date}`);
        const data = await req.data;
        commit("SET_UUIDSTATE", data);
        commit("SET_CARGANDO", false);
      } catch (error) {
        commit("SET_CARGANDO", false);
        console.log(error);
      }
    },
    getCuadroInversion: async ({ commit }, fechas) => {
      try {
        const req = await axiosInstance.get(
          `/api/dataapp/cuadroinversion?inputFechas=${fechas}`
        );
        const data = await req.data;
        return data;
      } catch (error) {
        console.log(error);
        return [];
      }
    },
  },
};

export default Informes;
