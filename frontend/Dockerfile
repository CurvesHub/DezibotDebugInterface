# Use a Node.js image
FROM node:18

# Set working directory
WORKDIR /app

# Copy package.json and yarn.lock
COPY package.json yarn.lock ./

# Install dependencies
RUN yarn install

# Copy the rest of the application files
COPY . .

# Expose the Nuxt development port
EXPOSE 3000

# Command to start Nuxt in development mode
CMD ["yarn", "dev", "-o"]
