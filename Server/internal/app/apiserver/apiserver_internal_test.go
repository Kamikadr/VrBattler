package apiserver

import (
	"net/http"
	"net/http/httptest"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestApiServer_HandleHello(test *testing.T) {
	api := New(NewConfig())
	recorder := httptest.NewRecorder()
	request, _ := http.NewRequest(http.MethodGet, "/hello", nil)
	api.hello().ServeHTTP(recorder, request)
	assert.Equal(test, recorder.Body.String(), "HELLO")
}
