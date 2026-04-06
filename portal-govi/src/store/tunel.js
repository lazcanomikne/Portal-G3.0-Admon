import { axiosInstance } from "../main";

const TunelBancario = {
  namespaced: true,
  state: () => ({
    resultado: [],
    datosInforme: {
      diferencias: [],
      rows: [],
      statistics: [],
    },
  }),
  getters: {
    doneRows: (state) => state.datosInforme,
    lenDif: (state) => state.datosInforme.diferencias.length,
    lenRows: (state) => state.datosInforme.rows.length,
    lenStats: (state) => state.datosInforme.statistics.length,
  },
  mutations: {
    SET_RESULTADOS: (state, datos) => {
      state.resultado = datos;
    },
    SET_INFORME: (state, datos) => {
      state.datosInforme = datos;
    },
  },
  actions: {
    postUpload: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        try {
          axiosInstance
            .post(`/api/dataapp/uploadtxt`, data, {
              headers: { "Content-Type": "multipart/form-data" },
            })
            .then((res) => {
              commit("SET_RESULTADOS", res.data);
              resolve(res);
            })
            .catch((err) => reject(err));
        } catch (error) {
          console.log(error);
        }
      });
    },
    postTunel: ({ commit }, info) => {
      return new Promise((resolve, reject) => {
        try {
          axiosInstance
            .post(`/api/dataapp/tunel`, info)
            .then((res) => resolve(res))
            .catch((err) => reject(err));
        } catch (error) {
          console.log(error);
          reject(error);
        }
      });
    },
    postUploadServicio: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        try {
          axiosInstance
            .post(`/api/dataapp/uploadtxtservicio`, data, {
              headers: { "Content-Type": "multipart/form-data" },
            })
            .then((res) => {
              commit("SET_RESULTADOS", res.data);
              resolve(res);
            })
            .catch((err) => reject(err));
        } catch (error) {
          console.log(error);
        }
      });
    },
    postTunelServicio: ({ commit }, info) => {
      return new Promise((resolve, reject) => {
        try {
          axiosInstance
            .post(`/api/dataapp/tunelservicio`, info)
            .then((res) => resolve(res))
            .catch((err) => reject(err));
        } catch (error) {
          console.log(error);
          reject(error);
        }
      });
    },
    getInforme: ({ commit }, fecha) => {
      return new Promise((resolve, reject) => {
        try {
          axiosInstance
            .post(`/api/dataapp/informe`, fecha)
            .then((res) => {
              commit("SET_INFORME", res.data);
              resolve(res);
            })
            .catch((err) => reject(err));
        } catch (error) {
          console.log(error);
          reject(error);
        }
      });
    },
  },
};

export default TunelBancario;
