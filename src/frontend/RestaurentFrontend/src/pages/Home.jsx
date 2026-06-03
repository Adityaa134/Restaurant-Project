import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { DishCard } from "../components/index";
import "../../src/index.css";

function Home() {
  const dishes = useSelector((state) => state.dishes.dishes);
  return (
    <div className="container mx-auto px-4 py-8">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 sm:gap-6">
        {dishes?.map((dish) => {
          return (
            <div key={dish.dishId}>
              <DishCard {...dish} />
            </div>
          );
        })}
      </div>
    </div>
  );
}

export default Home;
