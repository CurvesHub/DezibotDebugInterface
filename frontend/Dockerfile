ARG NODE_VERSION=22.0.0

FROM node:${NODE_VERSION}-slim AS base

ARG PORT=3000

WORKDIR /src

# ---------------------------- Build
FROM base AS build

COPY --link package.json yarn.lock .
RUN yarn install

COPY --link . .

RUN yarn build

# ---------------------------- Run
FROM base

ENV PORT=$PORT
ENV NODE_ENV=production

COPY --from=build /src/.output /src/.output
COPY --from=build /src/node_modules /src/node_modules

CMD [ "node", ".output/server/index.mjs" ]