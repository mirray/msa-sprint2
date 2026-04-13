import { ApolloServer } from '@apollo/server';
import { startStandaloneServer } from '@apollo/server/standalone';
import { buildSubgraphSchema } from '@apollo/subgraph';
import gql from 'graphql-tag';
import DataLoader from 'dataloader'

const typeDefs = gql`
  extend schema
    @link(
      url: "https://specs.apollo.dev/federation/v2.0"
      import: ["@key", "@shareable", "@override", "@external", "@requires", "@provides"]
    )

  extend type Booking @key(fields: "id") {
    id: ID!
    promoCode: String @external
    discountPercent: Int @external # @override(from: "booking-subgraph")
    hotel: Hotel! @external
    discount: DiscountInfo @requires(fields: "promoCode discountPercent hotel {id}")
  }
  
  type DiscountInfo  @key(fields: "code"){
    code: ID!
    isValid: Boolean
    originalDiscount: Int    
    finalDiscount: Int      
    description: String
    expiresAt: String
    applicableHotels: [ID!]!
  }
  type Hotel @key(fields: "id") { 
    id: ID! @external
  }
  type Query {
    validatePromoCode(code: String!, hotelId: ID): DiscountInfo!
    activePromoCodes: [DiscountInfo!]!
  }
`;

async function batchDiscounts(promos) {
  console.log("batchDiscounts")
  console.log(promos)
  var map = new Map();
  for (var id in promos) {
    try {
      map.set(promos[id], await getDiscount(promos[id]));
    } catch(error) {
      console.error('Request failed', error);
    }
  }
  return promos.map(code => map.get(code) || new Error(`No Discount found for ${code}`));;
}

async function getDiscount(code)
{
  console.log("get Discount")
  console.log(code)
  return {
    code: code,
    applyDiscount:15,
    description:"Test discount info",
    expiresAt: "2030-01-01",
    applicableHotels:["test-hotel-1"]
  }; 
} 

const discountLoader = new DataLoader(batchDiscounts)

function calculateDiscount(promocode, discount_percent, hotelId){
  return new Promise(async (resolve, reject) => {
    console.log("calculateDiscount")
    if (!promocode) return resolve(null);
    var discount = await discountLoader.load(promocode);
    discount.originalDiscount = discount_percent;
    discount.isValid = discount.applicableHotels.indexOf(hotelId)!==-1;
    discount.finalDiscount = discount.originalDiscount + discount.applyDiscount;
    return resolve(discount);
  });
}
//not really good in javascript. Probably should be solved some more efficient way
function calculateDiscountPersent(promocode, discount_percent, hotelId){
  return new Promise(async (resolve, reject) => {
    console.log("calculateDiscountPersent")
    if (!promocode) return resolve(0);
    var discount = await discountLoader.load(promocode);
    discount.originalDiscount = discount_percent;
    discount.isValid = discount.applicableHotels.indexOf(hotelId)!==-1;
    discount.finalDiscount = discount.originalDiscount + discount.applyDiscount;
    if (discount.isValid) return resolve(discount.finalDiscount);
    return resolve(discount_percent);
  });
}

const resolvers = {
  Booking: {
    discountPercent: (parent) => {
      console.log(parent);
      return calculateDiscountPersent(parent.promoCode, parent.discountPercent, parent.hotel.id)},
    discount: (parent) => {
      console.log(parent);
      return calculateDiscount(parent.promoCode, parent.discountPercent, parent.hotel.id)},
   
  },
  DiscountInfo: {
    __resolveReference: async ({ rep }) => {return await discountLoader.load(rep);
    },
    
  },
  Query: {
    validatePromoCode: async (_, req, __) => {  
      console.log(req);
      var discount = await discountLoader.load(req.code)
      discount.hotelId = req.hotelId
      return discount;
    },
    activePromoCodes: async(_, __, ___) => {return await discountLoader.loadMany(["TESTCODE1", "TESTCODE2"]);},
  },
};

const server = new ApolloServer({
  schema: buildSubgraphSchema([{ typeDefs, resolvers }]),
});

startStandaloneServer(server, {
  listen: { port: 4003 },
}).then(() => {
  console.log('✅ Promo subgraph ready at http://localhost:4003/');
});
