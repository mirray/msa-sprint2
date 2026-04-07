import { ApolloServer } from '@apollo/server';
import { startStandaloneServer } from '@apollo/server/standalone';
import { buildSubgraphSchema } from '@apollo/subgraph';
import gql from 'graphql-tag';
import * as grpc from '@grpc/grpc-js';
import * as protoLoader from '@grpc/proto-loader';

const packageDefinition = protoLoader.loadSync('booking.proto', {
      keepCase: true,
      longs: String,
      enums: String,
      defaults: true, // This ensures default values are visible
      oneofs: true
    });
const protoDescriptor = grpc.loadPackageDefinition(packageDefinition);
const BookingService = protoDescriptor.booking.BookingService;

const client = new BookingService('host.docker.internal:9090', grpc.credentials.createInsecure());

function getListOfBooking(user_id) {
    return new Promise((resolve, reject) => {
      client.ListBookings(
        { user_id: user_id },
        (err, response) => {
          if (err) {
            reject(err);
            return;
          }
          console.log('service:', JSON.stringify(response));

          resolve(response);
        }
      );
    });
  };



const typeDefs = gql`
  extend schema
    @link(
      url: "https://specs.apollo.dev/federation/v2.0"
      import: ["@key", "@shareable"]
    )
  type Booking @key(fields: "id") {
    id: ID!
    userId: String!
    hotel: Hotel
    promoCode: String
    discountPercent: Int
  }
  type Hotel @key(fields: "id") {
    id: ID!
  }
  type Query {
    bookingsByUser(userId: String!): [Booking]
  }

`;

const resolvers = {
  Query: {
    bookingsByUser: async (_, { userId }, { req }) => {
      if (userId != req.headers["userid"])
      {
        return []
      }
      var res = await getListOfBooking(userId);
      return res.bookings;
    }      
    
  },
  Booking: {
	  id: (parent) => parent.id,
    userId: (parent) => parent.user_id,
    hotel: (parent) => {return {__typeName: "Hotel", id: parent.hotel_id}},
    promoCode: (parent) => parent.promo_code,
    discountPercent: (parent) => parent.discount_percent,
  },
};

const server = new ApolloServer({
  schema: buildSubgraphSchema([{ typeDefs, resolvers }])
});

startStandaloneServer(server, {
  listen: { port: 4001 },
  context: async ({ req }) => ({req}),
}).then(() => {
  console.log('✅ Booking subgraph ready at http://localhost:4001/');
});
