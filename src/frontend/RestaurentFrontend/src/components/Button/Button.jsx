function Button({
    children,
    type='button',
    className='',
    bgColor='',
    textColor='',
    ...props
}) {
  return (
    <button className={`px-4 py-2 rounded-lg ${className} ${bgColor} ${textColor}`} {...props} type={type}>
        {children}
    </button>
  )
}

export default Button