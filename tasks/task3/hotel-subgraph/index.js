import { ApolloServer } from '@apollo/server';
import { startStandaloneServer } from '@apollo/server/standalone';
import { buildSubgraphSchema } from '@apollo/subgraph';
import gql from 'graphql-tag';
import DataLoader from 'dataloader'

const typeDefs = gql`
  extend schema
    @link(
      url: "https://specs.apollo.dev/federation/v2.0"
      import: ["@key", "@shareable"]
    )
  type Hotel @key(fields: "id") {
    id: ID!
    name: String
    city: String
    stars: Int
  }

  type Query {
    hotelsByIds(ids: [ID!]!): [Hotel]
  }
`;

async function batchHotels(ids) {
  var map = new Map();
  for (var id in ids) {
    try {
      map.set(ids[id], await getHotel(ids[id]));
    } catch(error) {
      console.error('Request failed', error);
    }
  }
  return ids.map(id => map.get(id) || new Error(`No hotel found for ${id}`));;
}

async function getHotel(id)
{
  
        console.log("resolve reference")
        console.log(id)
        const url = `http://host.docker.internal:8084/api/hotels/`+ id;
        console.log(url);
        const response = await fetch(url);
        if (!response.ok) throw new Error('HTTP error ' + response.status);
        const hotel = await response.json();
        return hotel;
    
} 

const hotelLoader = new DataLoader(batchHotels)

const resolvers = {
  Hotel: {
    id: (parent) => parent.id,
    name: (parent) => parent.description,
    city: (parent) => parent.city,
    stars: (parent) => Math.floor(parent.rating),
    __resolveReference: async ({ id }) => {return await hotelLoader.load(id)},
  },
  Query: {
    hotelsByIds: async (_, { ids }) => {  return await hotelLoader.loadMany(ids);},
  },
};

const server = new ApolloServer({
  schema: buildSubgraphSchema([{ typeDefs, resolvers }]),
});

startStandaloneServer(server, {
  listen: { port: 4002 },
}).then(() => {
  console.log('✅ Hotel subgraph ready at http://localhost:4002/');
});
