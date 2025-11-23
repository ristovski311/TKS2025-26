import dotenv from 'dotenv';
import jwt from 'jsonwebtoken';
dotenv.config();
const secretKey = process.env.JWT_SECRET;

const jwtService = {
    decodeToken(token) {
        return jwt.decode(token);
    },

    verifyJwt(token) {
        try {
            return jwt.verify(token, secretKey);
        } catch (error) {
            return null;
        }
    },

    extractRole(token) {
        const decoded = this.decodeToken(token);
        return decoded.user_role;
    }

};

export default jwtService;