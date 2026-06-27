import React, { useState, useRef, useEffect } from "react";
import { useSelector } from "react-redux";
import { DishCard } from "../components/index";
import dishService from "../services/dishService";
import "../../src/index.css";

const PRICE_OPTIONS = [
  { label: "All prices", minPrice: null, maxPrice: null },
  { label: "Under ₹150", minPrice: null, maxPrice: 149 },
  { label: "₹150 – ₹300", minPrice: 150, maxPrice: 300 },
  { label: "Above ₹300", minPrice: 301, maxPrice: null },
];

const RATING_OPTIONS = [
  { label: "Any rating", minRating: null },
  { label: "4+ stars", minRating: 4 },
  { label: "3+ stars", minRating: 3 },
];

function Home() {
  const allDishes = useSelector((state) => state.dishes.dishes);

  const [panelOpen, setPanelOpen] = useState(false);
  const [pendingPriceIdx, setPendingPriceIdx] = useState(0);
  const [pendingRatingIdx, setPendingRatingIdx] = useState(0);
  const [appliedPriceIdx, setAppliedPriceIdx] = useState(0);
  const [appliedRatingIdx, setAppliedRatingIdx] = useState(0);
  const [filteredDishes, setFilteredDishes] = useState(null);
  const [filtering, setFiltering] = useState(false);
  const [visible, setVisible] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  const filterBarRef = useRef(null);

  useEffect(() => {
    const check = () => setIsMobile(window.innerWidth < 768);
    check();
    window.addEventListener("resize", check);
    return () => window.removeEventListener("resize", check);
  }, []);


  useEffect(() => {
    if (panelOpen) {
      setPendingPriceIdx(appliedPriceIdx);
      setPendingRatingIdx(appliedRatingIdx);
      requestAnimationFrame(() => setVisible(true));
    } else {
      setVisible(false);
    }
  }, [panelOpen]);

  useEffect(() => {
    const onKey = (e) => {
      if (e.key === "Escape") closePanel();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, []);

  const closePanel = () => setPanelOpen(false);

  const callFilter = async (priceIdx, ratingIdx) => {
    const price = PRICE_OPTIONS[priceIdx];
    const rating = RATING_OPTIONS[ratingIdx];
    if (priceIdx === 0 && ratingIdx === 0) {
      setFilteredDishes(null);
      return;
    }
    try {
      setFiltering(true);
      const result = await dishService.FilterDishes({
        minPrice: price.minPrice,
        maxPrice: price.maxPrice,
        minRating: rating.minRating,
      });
      setFilteredDishes(Array.isArray(result) ? result : []);
    } catch (err) {
      console.error(err);
    } finally {
      setFiltering(false);
    }
  };

  const handleApply = async () => {
    setAppliedPriceIdx(pendingPriceIdx);
    setAppliedRatingIdx(pendingRatingIdx);
    closePanel();
    await callFilter(pendingPriceIdx, pendingRatingIdx);
  };

  const handleClearAll = async () => {
    setPendingPriceIdx(0);
    setPendingRatingIdx(0);
    setAppliedPriceIdx(0);
    setAppliedRatingIdx(0);
    setFilteredDishes(null);
    closePanel();
  };

  const removePrice = async () => {
    setAppliedPriceIdx(0);
    await callFilter(0, appliedRatingIdx);
  };

  const removeRating = async () => {
    setAppliedRatingIdx(0);
    await callFilter(appliedPriceIdx, 0);
  };

  const displayDishes = filteredDishes ?? allDishes ?? [];
  const activeCount =
    (appliedPriceIdx !== 0 ? 1 : 0) + (appliedRatingIdx !== 0 ? 1 : 0);

  const pendingActive = pendingPriceIdx !== 0 || pendingRatingIdx !== 0;

  const FilterContent = (
    <>
      <div className="flex items-center justify-between mb-4">
        <span className="text-sm font-semibold text-gray-900">Filters</span>
        {pendingActive && (
          <button
            onClick={handleClearAll}
            className="text-xs text-gray-400 hover:text-gray-700 transition-colors"
          >
            Clear all
          </button>
        )}
      </div>

      <div className="h-px bg-gray-100 mb-4" />

      <div className="mb-4">
        <p className="text-[10px] font-semibold text-gray-400 uppercase tracking-widest mb-2.5">
          Price
        </p>
        <div className="flex flex-col gap-2">
          {PRICE_OPTIONS.map((opt, idx) => (
            <button
              key={idx}
              onClick={() => setPendingPriceIdx(idx)}
              className={`text-sm px-4 py-2.5 rounded-xl border text-left font-medium transition-all duration-150
                ${
                  pendingPriceIdx === idx
                    ? "bg-gray-900 text-white border-gray-900"
                    : "bg-white text-gray-500 border-gray-200 hover:border-gray-300 hover:text-gray-700"
                }`}
            >
              {opt.label}
            </button>
          ))}
        </div>
      </div>

      <div className="h-px bg-gray-100 mb-4" />

      <div className="mb-2">
        <p className="text-[10px] font-semibold text-gray-400 uppercase tracking-widest mb-2.5">
          Rating
        </p>
        <div className="flex flex-col gap-2">
          {RATING_OPTIONS.map((opt, idx) => (
            <button
              key={idx}
              onClick={() => setPendingRatingIdx(idx)}
              className={`text-sm px-4 py-2.5 rounded-xl border text-left font-medium transition-all duration-150
                ${
                  pendingRatingIdx === idx
                    ? "bg-amber-50 text-amber-800 border-amber-300"
                    : "bg-white text-gray-500 border-gray-200 hover:border-gray-300 hover:text-gray-700"
                }`}
            >
              {idx !== 0 ? `⭐ ${opt.label}` : opt.label}
            </button>
          ))}
        </div>
      </div>

      <div className="mt-5">
        <button
          onClick={handleApply}
          className="w-full py-2.5 bg-gray-900 text-white text-sm font-semibold rounded-xl hover:bg-gray-700 active:scale-95 transition-all duration-150"
        >
          Apply filters
        </button>
      </div>
    </>
  );

  return (
    <div className="container mx-auto px-4 py-8">
      <div
        ref={filterBarRef}
        className="flex items-center gap-2 flex-wrap mb-0 pb-4 border-b border-gray-100"
      >
        <button
          onClick={() => setPanelOpen((p) => !p)}
          className={`flex items-center gap-2 text-sm px-4 py-2 rounded-lg border transition-colors flex-shrink-0
            ${
              panelOpen
                ? "bg-gray-900 border-gray-900 text-white"
                : "bg-white border-gray-200 text-gray-500 hover:border-gray-300 hover:text-gray-700"
            }`}
        >
          <svg className="w-4 h-4" viewBox="0 0 20 20" fill="none">
            <path
              d="M4 6h12M7 10h6M9 14h2"
              stroke="currentColor"
              strokeWidth="1.5"
              strokeLinecap="round"
            />
          </svg>
          Filters
          {activeCount > 0 && (
            <span
              className={`text-[11px] rounded-full px-2 py-0.5 ml-1 ${panelOpen ? "bg-white text-gray-900" : "bg-gray-900 text-white"}`}
            >
              {activeCount}
            </span>
          )}
        </button>

        {appliedPriceIdx !== 0 && (
          <span className="inline-flex items-center gap-1.5 text-sm px-3 py-1.5 rounded-full bg-blue-50 text-blue-800 border border-blue-200 flex-shrink-0">
            {PRICE_OPTIONS[appliedPriceIdx].label}
            <button
              onClick={removePrice}
              aria-label="Remove price filter"
              className="hover:text-blue-600 leading-none"
            >
              <svg className="w-3.5 h-3.5" viewBox="0 0 16 16" fill="none">
                <path
                  d="M4 4l8 8M12 4l-8 8"
                  stroke="currentColor"
                  strokeWidth="1.5"
                  strokeLinecap="round"
                />
              </svg>
            </button>
          </span>
        )}

        {appliedRatingIdx !== 0 && (
          <span className="inline-flex items-center gap-1.5 text-sm px-3 py-1.5 rounded-full bg-blue-50 text-blue-800 border border-blue-200 flex-shrink-0">
            ⭐ {RATING_OPTIONS[appliedRatingIdx].label}
            <button
              onClick={removeRating}
              aria-label="Remove rating filter"
              className="hover:text-blue-600 leading-none"
            >
              <svg className="w-3.5 h-3.5" viewBox="0 0 16 16" fill="none">
                <path
                  d="M4 4l8 8M12 4l-8 8"
                  stroke="currentColor"
                  strokeWidth="1.5"
                  strokeLinecap="round"
                />
              </svg>
            </button>
          </span>
        )}

        <span className="ml-auto text-sm text-gray-400 flex-shrink-0">
          {filtering
            ? "Filtering..."
            : activeCount > 0
              ? `${displayDishes.length} of ${allDishes?.length ?? 0} dishes`
              : `${allDishes?.length ?? 0} dishes`}
        </span>
      </div>

      <div className="relative">
        {panelOpen && !isMobile && (
          <>
            <div
              className="fixed inset-0 z-40 transition-opacity duration-300"
              style={{
                background: "rgba(0,0,0,0.15)",
                opacity: visible ? 1 : 0,
              }}
              onClick={closePanel}
            />

            <div
              className="fixed left-0 top-0 bottom-0 z-50 bg-white flex flex-col"
              style={{
                width: "min(280px, 80vw)",
                boxShadow: "2px 0 12px rgba(0,0,0,0.08)",
                transform: visible ? "translateX(0)" : "translateX(-100%)",
                transition: "transform 0.25s cubic-bezier(0.4,0,0.2,1)",
              }}
            >
              <div className="flex-1 overflow-y-auto px-5 pt-6 pb-2">
                {FilterContent}
              </div>
            </div>
          </>
        )}

        {panelOpen && isMobile && (
          <>
            <div
              className="fixed inset-0 z-40 transition-opacity duration-300"
              style={{
                background: "rgba(0,0,0,0.3)",
                opacity: visible ? 1 : 0,
              }}
              onClick={closePanel}
            />
            <div
              className="fixed bottom-0 left-0 right-0 z-50 bg-white rounded-t-3xl"
              style={{
                boxShadow: "0 -8px 32px rgba(0,0,0,0.12)",
                transform: visible ? "translateY(0)" : "translateY(100%)",
                transition: "transform 0.3s cubic-bezier(0.4,0,0.2,1)",
              }}
            >
              <div className="flex justify-center pt-3 pb-1">
                <div className="w-10 h-1 bg-gray-200 rounded-full" />
              </div>
              <div className="px-5 pt-3 pb-8">{FilterContent}</div>
            </div>
          </>
        )}

        <div className="pt-6">
          {filtering ? (
            <div className="flex flex-col items-center justify-center py-24 gap-3">
              <div className="w-8 h-8 border-[3px] border-gray-200 border-t-gray-700 rounded-full animate-spin" />
              <p className="text-sm text-gray-500">Applying filters...</p>
            </div>
          ) : displayDishes.length === 0 && activeCount > 0 ? (
            <div className="flex flex-col items-center justify-center py-24 gap-3 text-center">
              <span className="text-4xl">🔍</span>
              <p className="text-base font-medium text-gray-700">
                No dishes match your filters
              </p>
              <p className="text-sm text-gray-400">
                Try adjusting or clearing your filters
              </p>
              <button
                onClick={handleClearAll}
                className="mt-2 text-sm px-4 py-2 rounded-lg border border-gray-200 text-gray-500 hover:border-gray-300 hover:text-gray-700 transition-colors"
              >
                Clear filters
              </button>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 sm:gap-6">
              {displayDishes.map((dish) => (
                <div key={dish.dishId}>
                  <DishCard {...dish} />
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default Home;
