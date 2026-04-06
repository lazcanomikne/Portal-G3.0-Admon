import { axiosInstance } from "../main";

const Notas = {
  namespaced: true,
  actions: {
    postNotaDebito: ({ commit }, info) => {
      return new Promise((resolve, reject) => {
        try {
          let user = localStorage.getItem("user");
          let pass = localStorage.getItem("pass");
          info.Login = {
            UserName: user,
            Password: pass,
          };
          axiosInstance
            .post(`/api/dataapp/notadebito`, info)
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
  },
};

export default Notas;
