import react from '@vitejs/plugin-react';
import { defineConfig, type Plugin } from 'vite';

const contentSecurityPolicy = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; connect-src 'self'";

function contentSecurityPolicyOnBuild(): Plugin {
  return {
    name: 'mixer-content-security-policy',
    apply: 'build',
    transformIndexHtml: () => [{ tag: 'meta', attrs: { 'http-equiv': 'Content-Security-Policy', content: contentSecurityPolicy }, injectTo: 'head-prepend' }],
  };
}

export default defineConfig({
  root: 'src/renderer',
  base: './',
  plugins: [react(), contentSecurityPolicyOnBuild()],
  build: {
    outDir: '../../out/renderer',
    emptyOutDir: true,
  },
  server: {
    port: 5173,
    strictPort: true,
  },
});
