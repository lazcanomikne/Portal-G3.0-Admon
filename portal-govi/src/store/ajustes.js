import { axiosInstance } from "../main";

const AjustesModule = {
  namespaced: true,
  state: () => ({
    cedis: [],
  }),
  mutations: {
    setCedis: (state, datos) => {
      state.cedis = datos;
    },
  },
  actions: {
    getCedis: async ({ commit }) => {
      try {
        const req = await axiosInstance.get(`/api/dataapp/cedis`);
        const data = await req.data;
        commit("setCedis", data);
      } catch (error) {
        console.log(error);
      }
    },
    postAjuste: ({ commit }, info) => {
      return new Promise((resolve, reject) => {
        try {
          let user = localStorage.getItem("user");
          axiosInstance
            .post(`/api/dataapp/${info.Tipo}?u=${user}`, info)
            .then((res) => resolve(res))
            .catch((err) => reject(err));
        } catch (error) {
          console.log(error);
          reject(error);
        }
      });
    },
  },
};

export default AjustesModule;
