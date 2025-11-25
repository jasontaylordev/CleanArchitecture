// @ts-check
import eslint from "@eslint/js";
import ngrx from "@ngrx/eslint-plugin/v9";
import stylistic from "@stylistic/eslint-plugin";
import angular from "angular-eslint";
import eslintPluginPrettierRecommended from "eslint-plugin-prettier/recommended";
import tseslint from "typescript-eslint";

export default tseslint.config(
  {
    ignores: [
      ".angular/**",
      ".yarn/**",
      ".*", //Everything starting with a .
      "dist/**",
      "node_modules/**",
      "playwright-report/**",
      "**/generated/**",
      "src/app/shared/services/api.service.ts",
      "src/app/shared/services/signalr/**",
    ],
  },
  {
    files: ["**/*.ts"],
    extends: [
      eslint.configs.recommended,
      ...tseslint.configs.recommended,
      ...tseslint.configs.stylistic,
      ...angular.configs.tsRecommended,
      ...ngrx.configs.all,
    ],
    processor: angular.processInlineTemplates,
    plugins: {
      "@stylistic": stylistic,
    },
    rules: {
      "@angular-eslint/directive-selector": [
        "error",
        {
          type: "attribute",
          prefix: "ct",
          style: "camelCase",
        },
      ],
      "@angular-eslint/component-selector": [
        "error",
        {
          type: "element",
          prefix: "ct",
          style: "kebab-case",
        },
      ],
      "@stylistic/quotes": [
        "error",
        "double",
        {
          avoidEscape: true,
          allowTemplateLiterals: "always",
        },
      ],
      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          args: "all",
          argsIgnorePattern: "^_",
          caughtErrors: "all",
          caughtErrorsIgnorePattern: "^_",
          destructuredArrayIgnorePattern: "^_",
          varsIgnorePattern: "^_",
          ignoreRestSiblings: true,
        },
      ],
      "@typescript-eslint/no-non-null-asserted-optional-chain": "off",
    },
  },
  {
    files: ["**/*.html"],
    extends: [
      ...angular.configs.templateRecommended,
      ...angular.configs.templateAccessibility,
    ],
    rules: {
      "@angular-eslint/template/no-negated-async": "off",
      "@angular-eslint/template/eqeqeq": [
        "error",
        {
          allowNullOrUndefined: true,
        },
      ],
    },
  },
  eslintPluginPrettierRecommended,
  {
    files: ["**/*.ts"],
    rules: {
      curly: ["error", "all"],
    },
  }
);
