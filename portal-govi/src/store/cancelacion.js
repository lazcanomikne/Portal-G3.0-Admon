import { axiosInstance } from "../main";

const Cancelacion = {
  namespaced: true,
  actions: {
    postCancelacion: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        try {
          let user = localStorage.getItem("user");
          data.forEach((val) => (val["Usuario"] = user));
          axiosInstance
            .post(`/api/Cancelacion`, data)
            .then((res) => resolve(res))
            .catch((err) => {
              let res = {
                data: [
                  {
                    docEntry: 0,
                    cliente: err.data,
                    cantidad: 0,
                    precio: 0,
                  },
                ],
              };
              resolve(res);
            });
        } catch (error) {
          console.log(error);
          let res = {
            data: [
              {
                docEntry: 0,
                cliente: error,
                cantidad: 0,
                precio: 0,
              },
            ],
          };
          resolve(res);
        }
      });
    },
    putCancelacion: ({ commit }) => {
      return new Promise((resolve, reject) => {
        try {
          axiosInstance
            .put(`/api/Cancelacion`)
            .then((res) => resolve(res))
            .catch((err) => {
              reject(res);
            });
        } catch (error) {
          console.log(error);
        }
      });
    },
  },
};

export default Cancelacion;
