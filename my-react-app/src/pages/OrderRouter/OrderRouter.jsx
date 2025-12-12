import React from "react";
import { Navigate } from "react-router-dom";

import OrderPlacementPickUp from "../OrderPlacementPickUp/OrderPlacementPickUp";
//import OrderPlacementNotRegistered from "../OrderPlacementNotRegistered/OrderPlacementNotRegistered";
//import OrderPlacementRegistered from "../OrderPlacementRegistered/OrderPlacementRegistered";
//import OrderPlacementRegisteredUserAndOtherReceiver from "../OrderPlacementRegisteredUserAndOtherReceiver/OrderPlacementRegisteredUserAndOtherReceiver";

const OrderRouter = ({ user }) => {

  if (!user) return <OrderPlacementNotRegistered />;

  if (user.registered && user.selfReceiver) 
    return <OrderPlacementRegistered />;

  if (user.registered && !user.selfReceiver) 
    return <OrderPlacementRegisteredUserAndOtherReceiver />;

  return <OrderPlacementPickUp />;
};

export default OrderRouter;
