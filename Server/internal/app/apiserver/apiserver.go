package apiserver

import (
	"io"
	"net/http"

	"github.com/gorilla/mux"
	"github.com/sirupsen/logrus"
)

type ApiServer struct {
	config *Config
	logger *logrus.Logger
	router *mux.Router
}

func New(config *Config) *ApiServer {
	return &ApiServer{
		config: config,
		logger: logrus.New(),
		router: mux.NewRouter(),
	}
}

func (api *ApiServer) Start() error {

	if err := api.initializeLogger(); err != nil {
		return err
	}
	api.logger.Info("Logger initialized")

	api.initializeRouter()
	api.logger.Info("Router initialized")
	return http.ListenAndServe(api.config.BindAddress, api.router)
}

func (api *ApiServer) initializeLogger() error {
	level, err := logrus.ParseLevel(api.config.LogLevel)
	if err != nil {
		return err
	}
	api.logger.SetLevel(level)
	return nil
}

func (api *ApiServer) initializeRouter() {
	api.router.HandleFunc("/hello", api.hello())
}

func (api *ApiServer) hello() http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		io.WriteString(w, "HELLO")
	}
}
