function Container({ children }) {
  return (
    <div className="w-full max-w-7xl mx-auto px-4 py-6">
      <div className="bg-white rounded-lg shadow-lg p-6 border border-gray-200">
        {children}
      </div>
    </div>

  )
}

export default Container