import { axiosInstance } from "../main";

const Dispersion = {
  namespaced: true,
  state: () => ({
    sociedades: [],
    sucursales: [],
    cuentas: [],
    transferencias: [],
    pasivos: [],
    transferenciasDispersion: [],
    services: [],
  }),
  mutations: {
    SET_SOCIEDADES: (state, datos) => {
      state.sociedades = datos;
    },
    SET_SUCURSALES: (state, datos) => {
      state.sucursales = [];
      state.sucursales = datos;
    },
    SET_CUENTAS: (state, datos) => {
      state.cuentas = [];
      state.cuentas = datos;
    },
    SET_TRANSFERS: (state, datos) => {
      state.transferencias = [];
      state.transferencias = datos;
    },
    SET_PASIVOS: (state, datos) => {
      state.pasivos = [];
      state.pasivos = datos;
    },
    SET_TRANSFERSDISPERSION: (state, datos) => {
      state.transferenciasDispersion = [];
      state.transferenciasDispersion = datos;
    },
    SET_SERVICES: (state, datos) => {
      state.services = [];
      state.services = datos;
    },
  },
  actions: {
    limpiar: ({ commit }) => {
      commit("SET_TRANSFERS", []);
      commit("SET_TRANSFERSDISPERSION", []);
    },
    getSociedades: ({ commit }) => {
      axiosInstance.get(`/api/dataapp/sociedades`).then((res) => {
        commit("SET_SOCIEDADES", res.data);
      });
    },
    getSucursales: ({ commit }, sociedad) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(`/api/dataapp/sucursales?sociedad=${sociedad}`)
          .then((res) => {
            commit("SET_SUCURSALES", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    getCuentas: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(
            `/api/dataapp/cuentas?sociedad=${data.sociedad}&sucursal=${data.sucursal}`
          )
          .then((res) => {
            commit("SET_CUENTAS", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    getTransfers: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(
            `/api/dataapp/transferencias?sociedad=${data.sociedad}&sucursal=${data.sucursal}&cuenta=${data.cuenta}&operacion=${data.operacion}&year=${data.year}`
          )
          .then((res) => {
            commit("SET_TRANSFERS", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    getPasivos: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(
            `/api/dataapp/pasivos?sociedad=${data.sociedad}&sucursal=${data.sucursal}&cuenta=${data.cuenta}`
          )
          .then((res) => {
            commit("SET_PASIVOS", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    postPasivos: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        try {
          let user = localStorage.getItem("user");
          let pass = localStorage.getItem("pass");
          axiosInstance
            .post(
              `/api/dataapp/pasivos?u=${user}&p=${pass}&sociedad=${data.sociedad}`,
              data.info
            )
            .then((res) => resolve(res.data))
            .catch((err) => reject(err.response));
        } catch (error) {
          console.log(error);
          reject(error);
        }
      });
    },
    getAllTransfers: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(`/api/dataapp/transferencias`)
          .then((res) => {
            commit("SET_TRANSFERS", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    getAllTransfersDispersion: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(
            `/api/dataapp/transferenciasDispersion?sociedad=${data.sociedad}&fecha1=${data.fecha1}&fecha2=${data.fecha2}`
          )
          .then((res) => {
            commit("SET_TRANSFERSDISPERSION", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    getAllServices: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .get(`/api/dataapp/servicios`)
          .then((res) => {
            commit("SET_SERVICES", res.data);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response.data);
          });
      });
    },
    updateDispersion: ({ commit }, data) => {
      let user = localStorage.getItem("user");
      let pass = localStorage.getItem("pass");
      let postUrl = `/api/dataapp/updateDispersion?sociedad=${data.sociedad}&u=${user}&p=${pass}`;
      return new Promise((resolve, reject) => {
        axiosInstance
          .post(postUrl, data.transferencias)
          .then((res) => resolve(res.data))
          .catch((err) => reject(err));
      });
    },
    generarTxtxLote: ({ commit }, data) => {
      let user = localStorage.getItem("user");
      let pass = localStorage.getItem("pass");
      let postUrl = `/api/dataapp/transferencias?sociedad=${data.sociedad}&sucursal=${data.sucursal}&operacion=${data.operacion}&u=${user}&p=${pass}`;
      return new Promise((resolve, reject) => {
        fetch(postUrl, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(data.transferencias),
        })
          .then((res) => {
            let filename = res.headers.get("filename");
            resolve({ url: "", filename });
          })
          .catch((err) => {
            reject(err);
          });
      });
    },
    generarTxtUnoxUno: ({ commit }, data) => {
      let user = localStorage.getItem("user");
      let pass = localStorage.getItem("pass");
      let postUrl = `/api/dataapp/transferenciasbyone?u=${user}&p=${pass}&g=${data.g}`;
      return new Promise((resolve, reject) => {
        axiosInstance
          .post(postUrl, data.transferencias)
          .then((res) => resolve(res.data))
          .catch((err) => reject(err));
      });
    },
    generarTxtServices: ({ commit }, data) => {
      let user = localStorage.getItem("user");
      let pass = localStorage.getItem("pass");
      let postUrl = `/api/dataapp/servicios?u=${user}&p=${pass}&g=${data.g}`;
      return new Promise((resolve, reject) => {
        axiosInstance
          .post(postUrl, data.services)
          .then((res) => resolve(res.data))
          .catch((err) => reject(err));
      });
    },
    postPagosFiliales: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        try {
          let user = localStorage.getItem("user");
          let pass = localStorage.getItem("pass");
          axiosInstance
            .post(
              `/api/dataapp/pagofiliales?u=${user}&p=${pass}&sociedad=${data.sociedad}&sucursal=${data.sucursal}&proveedor=${data.proveedor}`,
              data.info
            )
            .then((res) => resolve(res))
            .catch((err) => reject(err.response));
        } catch (error) {
          console.log(error);
          reject(error);
        }
      });
    },
  },
};

export default Dispersion;
