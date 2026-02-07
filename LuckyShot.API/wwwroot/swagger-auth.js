(() => {
  const STORAGE_KEY = "swagger_auth_token";

  function restoreToken(ui) {
    const token = localStorage.getItem(STORAGE_KEY);
    if (token) {
      ui.preauthorizeApiKey("Bearer", token);
    }
  }

  function ensureLoginSection(ui) {
    const authContainer = document.querySelector(".auth-container");
    if (!authContainer || document.getElementById("swagger-login-section")) return;

    const section = document.createElement("div");
    section.id = "swagger-login-section";
    section.className = "swagger-login-section";
    section.innerHTML = `
      <div class="swagger-login-title">Login</div>
      <label>Email</label>
      <input id="swagger-login-email" type="email" placeholder="email@exemplo.com" />
      <label>Senha</label>
      <input id="swagger-login-password" type="password" placeholder="••••••••" />
      <div class="swagger-login-actions">
        <button id="swagger-login-submit" class="btn authorize">Entrar</button>
      </div>
      <div id="swagger-login-error" class="swagger-login-error"></div>
      <hr />
      <div class="swagger-login-hint">Ou cole um token manualmente abaixo.</div>
    `;

    authContainer.prepend(section);
    document.getElementById("swagger-login-submit").onclick = () => submitLogin(ui);
  }

  async function submitLogin(ui) {
    const email = document.getElementById("swagger-login-email").value;
    const password = document.getElementById("swagger-login-password").value;
    const errorEl = document.getElementById("swagger-login-error");
    errorEl.innerText = "";

    try {
      const response = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
      });

      if (!response.ok) {
        const data = await response.json().catch(() => ({}));
        throw new Error(data.message || "Falha ao autenticar.");
      }

      const data = await response.json();
      const token = data.accessToken;
      if (!token) throw new Error("Token não encontrado na resposta.");

      ui.preauthorizeApiKey("Bearer", token);
      localStorage.setItem(STORAGE_KEY, token);
      errorEl.innerText = "Autorizado com sucesso.";
    } catch (err) {
      errorEl.innerText = err.message || "Erro inesperado.";
    }
  }

  function observeAuthorizeModal(ui) {
    const observer = new MutationObserver(() => {
      const wrapper = document.querySelector(".auth-wrapper");
      if (wrapper) {
        ensureLoginSection(ui);
      }
    });

    observer.observe(document.body, { childList: true, subtree: true });
  }

  function waitForUi() {
    const interval = setInterval(() => {
      const ui = window.ui;
      if (ui) {
        clearInterval(interval);
        restoreToken(ui);
        observeAuthorizeModal(ui);
      }
    }, 300);
  }

  window.addEventListener("load", () => {
    waitForUi();
  });
})();